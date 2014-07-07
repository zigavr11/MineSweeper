using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MineSweeper
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int length;
        bool doubleClick;
        ButtonState current, before;
        Mouse Mouse;
        //Arrays
        int[,] array;
        bool[,] hidden;
        bool[,] flagged;
        Rectangle[,] recs;

        Rectangle mouse;
        List<Texture2D> textures;

        Random randomNum;
        int xMargin, yMargin;

        //Za flooding algoritm
        List<int[]> floodingArrays;
        int[] arrayOfInts;

        int timer;
        int intArrayCounter, listCounter;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 600;
            IsMouseVisible = true;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            randomNum = new Random();
            length = 12; // 12 - 2 = 10 => Matrica velika 10x10
            doubleClick = false;
            timer = 0;

            before = Mouse.GetState().RightButton;
            current = Mouse.GetState().LeftButton;

            array = new int[length, length];
            recs = new Rectangle[length, length];
            hidden = new bool[length, length];
            flagged = new bool[length, length];

            floodingArrays = new List<int[]>();
            arrayOfInts = new int[16];
            intArrayCounter = 0;
            listCounter = 0;

            mouse = new Rectangle(0, 0, 2, 2);
            xMargin = 50;
            yMargin = 50;
            textures = new List<Texture2D>();

            //Calling methods
            mineGenerator(length - 2, array, 10);
            loadArray(array, recs);
            LoadContent();

            base.Initialize();
        }

        public void loadArray(int[,] array, Rectangle[,] recs)
        {
            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < length; y++)
                {
                    if(x == 0 || y == 0 || x == length - 1 || y == length - 1){
                        recs[x, y] = new Rectangle(x * xMargin, y * yMargin, xMargin - 10, yMargin - 10);
                        array[x, y] = 12;
                        hidden[x,y] = true;
                        Console.WriteLine("x: " + x + "y: " + y );
                    }
                    else{
                        recs[x, y] = new Rectangle(x * xMargin, y * yMargin, xMargin - 10, yMargin - 10);
                        hidden[x, y] = false; // Ce das na true bos vidu celo tabelo z stevilkami, minami itd, false da vse skrijes
                        flagged[x, y] = false;
                        if (array[x, y] != 9)
                            array[x, y] = countMines(x, y);
                        else
                            hidden[x, y] = false; // da mine sam vidim kje so
                    }
                }
            }
        }

        public int countMines(int x, int y)
        {
            int count = 0;
            if (array[x - 1, y - 1] == 9) // gor levo
            {
                count++;
            }
            if (array[x - 1, y] == 9) // gor
            {
                count++;
            }
            if (array[x - 1, y + 1] == 9) // gor desno
            {
                count++;
            }
            if (array[x, y - 1] == 9) //levo
            {
                count++;
            }
            if (array[x, y + 1] == 9) //desno
            {
                count++;
            }
            if (array[x + 1, y - 1] == 9) //dol levo
            {
                count++;
            }
            if (array[x + 1, y] == 9) // dol
            {
                count++;
            }
            if (array[x + 1, y + 1] == 9) // dol desno
            {
                count++;
            }
            return count;
        }

        public void mineGenerator(int length, int[,] array, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int x = randomNum.Next(1, length + 1);
                int y = randomNum.Next(1, length + 1);
                if (array[x, y] != 9)
                {
                    array[x, y] = 9;
                }
                else if (array[x, y] == 9)
                    i--;
            }
        }

        protected override void LoadContent()
        {
            textures.Add(Content.Load<Texture2D>("whiteRectangle")); //0
            textures.Add(Content.Load<Texture2D>("1"));
            textures.Add(Content.Load<Texture2D>("2"));
            textures.Add(Content.Load<Texture2D>("3"));
            textures.Add(Content.Load<Texture2D>("4"));
            textures.Add(Content.Load<Texture2D>("5")); //5
            textures.Add(Content.Load<Texture2D>("6"));
            textures.Add(Content.Load<Texture2D>("7"));
            textures.Add(Content.Load<Texture2D>("8"));
            textures.Add(Content.Load<Texture2D>("bomb")); //9
            textures.Add(Content.Load<Texture2D>("flag")); //10
            textures.Add(Content.Load<Texture2D>("grayRectangle")); //11
            textures.Add(Content.Load<Texture2D>("blackRectangle")); //12
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            mouse.X = Mouse.GetState().X;
            mouse.Y = Mouse.GetState().Y;
            checkDoubleclick(gameTime);
            mouseIntersection(mouse);
            base.Update(gameTime);
        }

        public void checkDoubleclick(GameTime gameTime)
        {
            timer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            current = MouseState();
            before = current;
            Mouse = Mouse.GetState().RightButton;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)

                if (doubleClick)
                    Console.Write("Double Click");
        }

        ButtonState MouseState()
        {
            return ButtonState.Pressed;
        }

        public void mouseIntersection(Rectangle mouse)
        {
            for (int x = 1; x < length - 1; x++)
            {
                for (int y = 1; y < length - 1; y++)
                {
                    if (!doubleClick && mouse.Intersects(recs[x, y]) && Mouse.GetState().LeftButton == ButtonState.Pressed && array[x, y] != 9 && !flagged[x, y]) // Navadn klik ki razkrije polje, ce ni mina
                    {
                        hidden[x, y] = true;
                        if (array[x, y] == 0)
                        {
                            floodingAlgorithem(x, y);
                        }
                        Thread.Sleep(200);
                    }
                    else if(doubleClick)
                    {

                    }
                    else if (array[x, y] == 9 && Mouse.GetState().LeftButton == ButtonState.Pressed) // Game over ker mino zadane
                    {
                        //Game Over
                    }
                    else if (mouse.Intersects(recs[x, y]) && Mouse.GetState().RightButton == ButtonState.Pressed && !flagged[x, y] && !hidden[x, y]) // Da postavi zastavo na oznaceno polje
                    {
                        flagged[x, y] = true;
                        Thread.Sleep(200);
                    }
                    else if (mouse.Intersects(recs[x, y]) && Mouse.GetState().RightButton == ButtonState.Pressed && flagged[x, y] && !hidden[x, y]) // Da odstrani zastavo iz polja
                    {
                        flagged[x, y] = false;
                        Thread.Sleep(200);
                    }
                }
            }

        }

        public void fillArray(int x, int y)
        {
            intArrayCounter = 0;
            //Levo gor
            if (array[x - 1, y - 1] == 0 && !hidden[x - 1, y - 1])
            {
                arrayOfInts[intArrayCounter] = x - 1;
                arrayOfInts[intArrayCounter + 1] = y - 1;
                intArrayCounter += 2;
                hidden[x - 1, y - 1] = true;
            }
            else
                hidden[x - 1, y - 1] = true;
            //Sredina gor
            if (array[x - 1, y] == 0 && !hidden[x - 1, y])
            {
                arrayOfInts[intArrayCounter] = x - 1;
                arrayOfInts[intArrayCounter + 1] = y;
                intArrayCounter += 2;
                hidden[x - 1, y] = true;
            }
            else
                hidden[x - 1, y] = true;
            //Desno gor
            if (array[x - 1, y + 1] == 0 && !hidden[x - 1, y + 1])
            {
                arrayOfInts[intArrayCounter] = x - 1;
                arrayOfInts[intArrayCounter + 1] = y + 1;
                intArrayCounter += 2;
                hidden[x - 1, y + 1] = true;
            }
            else
                hidden[x - 1, y + 1] = true;
            //Sredina levo
            if (array[x, y - 1] == 0 && !hidden[x, y - 1])
            { 
                arrayOfInts[intArrayCounter] = x;
                arrayOfInts[intArrayCounter + 1] = y - 1;
                intArrayCounter += 2;
                hidden[x, y - 1] = true;
            }
            else
                hidden[x, y - 1] = true;
            //Sredina desno
            if (array[x, y + 1] == 0 && !hidden[x, y + 1])
            {
                arrayOfInts[intArrayCounter] = x;
                arrayOfInts[intArrayCounter + 1] = y + 1;
                intArrayCounter += 2;
                hidden[x, y + 1] = true;
            }
            else
                hidden[x, y + 1] = true;
            //Levo dol
            if (array[x + 1, y - 1] == 0 && !hidden[x + 1, y - 1])
            {
                arrayOfInts[intArrayCounter] = x + 1;
                arrayOfInts[intArrayCounter + 1] = y - 1;
                intArrayCounter += 2;
                hidden[x + 1, y - 1] = true;
            }
            else
                hidden[x + 1, y - 1] = true;
            //Sredina dol
            if (array[x + 1, y] == 0 && !hidden[x + 1, y])
            {
                arrayOfInts[intArrayCounter] = x + 1;
                arrayOfInts[intArrayCounter + 1] = y;
                intArrayCounter += 2;
                hidden[x + 1, y] = true;
            }
            else
                hidden[x + 1, y] = true;
            //Desno dol
            if (array[x + 1, y + 1] == 0 && !hidden[x + 1, y + 1])
            {
                arrayOfInts[intArrayCounter] = x + 1;
                arrayOfInts[intArrayCounter + 1] = y + 1;
                intArrayCounter += 2;
                hidden[x + 1, y + 1] = true;
            }
            else
                hidden[x + 1, y + 1] = true;

        }

        public void clearArray(int[] Array)
        {
            for (int i = 0; i < Array.Length; i++)
            {
                Array[i] = 0;
            }
        }
        
        public void floodingAlgorithem(int xk, int yk)
        {
            //Zacetni majnsi algoritm ki doda v arrayofint vse vrednosti ki so okrog xk, yk (tam kje kliknemo)
            fillArray(xk,yk); // Ta metoda pregleda vse elemente in ce imajo vrednost 0 in se niso odkriti intArrayCounter povecujem
            floodingArrays.Add(new int[intArrayCounter]); //Inicialziacije nove tabele
            for (int i = 0; i < intArrayCounter; i++) //Dodamo vse vrednosti iz arrayofints tabele v floodingArrays tabelo
            {
                floodingArrays[0][i] = arrayOfInts[i];
            }
            clearArray(arrayOfInts);

            for (int i = 0; i < floodingArrays.Count(); i++ ) // Gre tolikokrat kolikor imamo tabel v lsitu
            {
                for (int j = 0; j < floodingArrays[i].Count(); j += 2 ) // posle v fillArray x in y od vsake tocke
                {
                    fillArray(floodingArrays[i][j], floodingArrays[i][j + 1]); // Razkrije in presteje koliko polj ima vrednost 0
                    if(intArrayCounter > 0) // polje ki ima vrednost 0 pomeni da intArrayCounter dobi vrednost +2, torej ce ni nic polj okrog bo counter imel vrednost 0
                    {
                        floodingArrays.Add(new int[intArrayCounter]); // Inicialziramo novo tabelo 
                        for (int k = 0; k < intArrayCounter; k++) //Napolnimo novo tabelo z vrednostmi ki jih dodamo v fillArray tabeli ArrayofInts
                        {
                            floodingArrays[floodingArrays.Count - 1][k] = arrayOfInts[k]; //Dodajanje
                        }
                        clearArray(arrayOfInts); //resetiram tabelo tako da ima vednosti 0, pomoje ni potrebno da to naredim ampak sem vseen :D
                    }
                }
            }
            //Resetiram vse zato da naslednjic ko kliknem na polje se ponovno vse zacne
            floodingArrays.Clear();
            clearArray(arrayOfInts);

            
            
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < length; y++)
                {

                    spriteBatch.Draw(textures[11], recs[x, y], Color.White);
                    if (hidden[x, y] == true)
                    {
                        switch (array[x, y])
                        {
                            case 0: spriteBatch.Draw(textures[0], recs[x, y], Color.White); ; break;
                            case 1: spriteBatch.Draw(textures[1], recs[x, y], Color.White); ; break;
                            case 2: spriteBatch.Draw(textures[2], recs[x, y], Color.White); ; break;
                            case 3: spriteBatch.Draw(textures[3], recs[x, y], Color.White); ; break;
                            case 4: spriteBatch.Draw(textures[4], recs[x, y], Color.White); ; break;
                            case 5: spriteBatch.Draw(textures[5], recs[x, y], Color.White); ; break;
                            case 6: spriteBatch.Draw(textures[6], recs[x, y], Color.White); ; break;
                            case 7: spriteBatch.Draw(textures[7], recs[x, y], Color.White); ; break;
                            case 8: spriteBatch.Draw(textures[8], recs[x, y], Color.White); ; break;
                            case 9: spriteBatch.Draw(textures[9], recs[x, y], Color.White); ; break;
                            case 11: spriteBatch.Draw(textures[11], recs[x, y], Color.White); ; break;
                            case 12: spriteBatch.Draw(textures[12], recs[x, y], Color.White); ; break;
                        }
                    }
                    if (flagged[x, y])
                    {
                        spriteBatch.Draw(textures[10], recs[x, y], Color.White);
                    }
                }
            }
            spriteBatch.Draw(textures[0], mouse, Color.Red);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
