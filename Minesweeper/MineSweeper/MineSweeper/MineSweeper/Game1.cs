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
        //Arrays
        int[,] array;
        bool[,] hidden;
        bool[,] flagged;
        Rectangle[,] recs;

        Rectangle mouse;
        List<Texture2D> textures;
        Random randomNum;
        int xMargin, yMargin;

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
            
            array = new int[length, length];
            recs = new Rectangle[length, length];
            hidden = new bool[length, length];
            flagged = new bool[length, length];

            mouse = new Rectangle(0, 0, 2, 2);
            xMargin = 50;
            yMargin = 50;
            textures = new List<Texture2D>();

            //Calling methods
            mineGenerator(length -2,array, 10);
            loadArray(array, recs);
            LoadContent();

            base.Initialize();
        }

        public void loadArray(int[,] array, Rectangle[,] recs)
        {
            for (int x = 1; x < length -1; x++)
            {
                for (int y = 1; y < length -1; y++)
                {
                    recs[x, y] = new Rectangle(x * xMargin , y * yMargin, xMargin - 10, yMargin - 10);
                    hidden[x, y] = true; // Ce das na true bos vidu celo tabelo z stevilkami, minami itd, false da vse skrijes
                    flagged[x, y] = false;
                    if(array[x,y] != 9)
                        array[x, y] = countMines(x,y);
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

        public void mineGenerator(int length,int [,] array, int count)
        {
            for(int i = 0; i < count ; i++){
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
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            mouse.X = Mouse.GetState().X;
            mouse.Y = Mouse.GetState().Y;

            mouseIntersection(mouse);
            base.Update(gameTime);
        }

        public void mouseIntersection(Rectangle mouse)
        {
            for (int x = 1; x < length-1; x++)
            {
                for (int y = 1; y < length-1; y++)
                {
                    if (mouse.Intersects(recs[x, y]) && Mouse.GetState().LeftButton == ButtonState.Pressed && array[x,y] !=9 && !flagged[x,y])
                    {
                        hidden[x, y] = true;
                    }
                    else if(array[x,y] == 9 && Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        //Game Over
                    }
                    else if (mouse.Intersects(recs[x, y]) && Mouse.GetState().RightButton == ButtonState.Pressed && !flagged[x,y] && !hidden[x,y])
                    {
                        flagged[x, y] = true;
                        Thread.Sleep(100);
                    }
                    else if (mouse.Intersects(recs[x, y]) && Mouse.GetState().RightButton == ButtonState.Pressed && flagged[x, y] && !hidden[x, y])
                    {
                        flagged[x, y] = false;
                        Thread.Sleep(100);
                    }
                }
            }

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            for (int x = 1; x < length - 1; x++)
            {
                for (int y = 1; y < length - 1; y++)
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
