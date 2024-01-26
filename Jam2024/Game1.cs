using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Jam2024
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private AnimatedTexture left_hand = new AnimatedTexture(Vector2.Zero, 0, 1, 0);
        private AnimatedTexture right_hand = new AnimatedTexture(Vector2.Zero, 0, 1, 0);
        private KeyboardState ks,oldks;
        private MouseState ms, oldms;
        private SpriteBatch _spriteBatch;
        private Texture2D circle, play_button, reset_button, tutorial, blockbar;
        private SpriteFont overfont, gameplayfont;
        bool opentutorial = false;  //reset
        private ScreenState screen;
        private Random rnd = new Random();
        private bool gameplay_start = false, allow_gameopen = true, allow_soundend = true;    //reset
        private List<ClickBu> clickcircle = new List<ClickBu>();    //reset
        private float elapsed = 0;   //reset
        private int tool = 0;   //reset

        private Rectangle b_play, b_reset, tutorialbox, healthbar;

        private float max_health = 100;
        private float health = 100;   //reset
        private float time = 0;   //reset
        private float timeforhealth = 0;   //reset
        private float timestop = 1;   //reset
        private float timeforcircle = 0;   //reset
        private float timecreate = 1;    //reset
        private float timebeforeend = 2; //reset
        private float timebeforestart = 4; //reset
        private float timebeforeover = 3.5f;
        private int minute = 0, second = 0;   //reset

        private int highesttime = 0;

        private string filepath;
        private FileStream file;
        private BinaryReader reader;
        private BinaryWriter writer;

        static public List<SoundEffect> sEffect = new List<SoundEffect>();
        private int main_volume_effect = 1;
        enum ScreenState
        {
            menu,
            gameplay,
            end
        };

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 800;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            screen = ScreenState.menu; 
            filepath = Path.Combine(@"Content/data/highest.bin");
            file = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            reader = new BinaryReader(file);
            highesttime = reader.ReadInt32();
            reader.Close();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            left_hand.Load("testtexture", 1, 1, 1, 1, Content);
            right_hand.Load("testtexture", 1, 1, 1, 1, Content);
            circle = Content.Load<Texture2D>("testtexture");
            play_button = Content.Load<Texture2D>("testtexture");
            reset_button = Content.Load<Texture2D>("testtexture");
            tutorial = Content.Load<Texture2D>("testtexture");
            blockbar = Content.Load<Texture2D>("testtexture");
            overfont = Content.Load<SpriteFont>("sfont");
            gameplayfont = Content.Load<SpriteFont>("gameplayfont");
            sEffect.Add(Content.Load<SoundEffect>("Sound/OpenGamePlay"));     //0
            sEffect.Add(Content.Load<SoundEffect>("Sound/hurt"));     //1
            sEffect.Add(Content.Load<SoundEffect>("Sound/death"));     //2
            SoundEffect.MasterVolume = main_volume_effect;
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            ks = Keyboard.GetState();
            ms = Mouse.GetState();
            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (screen)
            {
                case ScreenState.menu:
                    update_menu(gameTime); break;
                case ScreenState.gameplay:
                    update_gameplay(gameTime); break;
                case ScreenState.end:
                    update_end(gameTime); break;
            }

            oldks = ks;
            oldms = ms;

            //reset highest
            if (ks.IsKeyDown(Keys.F10))
            {
                highesttime = 0;
                file = new FileStream(filepath, FileMode.Create, FileAccess.Write);
                writer = new BinaryWriter(file);
                writer.Write((int)0);
                writer.Flush();
                writer.Close();
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            switch (screen)
            {
                case ScreenState.menu:
                    draw_menu(gameTime); break;
                case ScreenState.gameplay:
                    draw_gameplay(gameTime); break;
                case ScreenState.end:
                    draw_end(gameTime); break;
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        //Updateeeeeeeeeeeeeeeeeeeeeeeeeeeee
        protected void update_menu(GameTime gameTime)
        {
            tutorialbox = new Rectangle(50, 50, 500, 400);
            if (!opentutorial)
            {
                b_play = new Rectangle(400, 400, 100, 70);
                if (b_play.Contains(ms.X, ms.Y) && ms.LeftButton == ButtonState.Pressed && oldms.LeftButton == ButtonState.Released)
                {
                    opentutorial = true;
                }
            }
            else
            {
                b_play = new Rectangle(450, 600, 80, 60);
                if (b_play.Contains(ms.X, ms.Y) && ms.LeftButton == ButtonState.Pressed && oldms.LeftButton == ButtonState.Released)
                {
                    screen = ScreenState.gameplay;
                }
            }
            if(highesttime >= 60)
            {
                minute = (int)(highesttime / 60);
                second = (int)(highesttime % 60);
            }
            else
            {
                minute = (int)0;
                second = (int)highesttime;
            }
        }

        protected void update_gameplay(GameTime gameTime)
        {
            if(timebeforestart > 0)
            {
                if (allow_gameopen)
                {
                    var instance = sEffect[0].CreateInstance();
                    instance.Volume = 0.7f;
                    instance.Play();
                    allow_gameopen = false;
                }
                timebeforestart -= elapsed;
            }
            else
            {
                
                gameplay_start = true;
            }

            if (health <= 0)
            {
                gameplay_start = false;
                if (allow_soundend)
                {
                    var instance = sEffect[2].CreateInstance();
                    instance.Volume = 0.5f;
                    instance.Play();
                    allow_soundend = false;
                }
                if(timebeforeover > 0)
                {
                    timebeforeover -= elapsed;
                }
                else
                {
                    screen = ScreenState.end;
                }
            }
            if(health > 100)
            {
                health = 100;
            }
            
            healthbar = new Rectangle(20, 5, (int)(400*(health/max_health)), 20);
            if (gameplay_start)
            {
                timeforhealth += (float)gameTime.ElapsedGameTime.TotalSeconds;
                timeforcircle += (float)gameTime.ElapsedGameTime.TotalSeconds;
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                timestop = 1 - (time / 90);
                timecreate = 2 - (time / 45);
                if (timecreate <= 0.7f)
                {
                    timecreate = 0.7f;
                }
                if (timestop <= 0.01f)
                {
                    timestop = 0.01f;
                }
                if (timeforhealth >= timestop)
                {
                    health -= 1;
                    timeforhealth = 0;
                }

                if(timeforcircle >= timecreate)
                {
                    clickcircle.Add(new ClickBu(circle, rnd.Next(1, 5), new Vector2(rnd.Next(50, 1100), rnd.Next(50, 400))));
                    timeforcircle = 0;
                }
                //change tool
                if (ks.IsKeyDown(Keys.D1))
                {
                    tool = 1;
                }
                else if (ks.IsKeyDown(Keys.D2))
                {
                    tool = 2;
                }
                else if (ks.IsKeyDown(Keys.D3))
                {
                    tool = 3;
                }
                else if (ks.IsKeyDown(Keys.D4))
                {
                    tool = 4;
                }

                //sound & animate
                if(tool == 1)
                {

                }
                else if(tool == 2)
                {

                }
                else if (tool == 3)
                {

                }
                else if (tool == 4)
                {

                }


                foreach (ClickBu minicircle in clickcircle) //button show
                {
                    minicircle.Update(elapsed);
                    if (minicircle.hitbox.Contains(ms.X, ms.Y))
                    {
                        if (ms.LeftButton == ButtonState.Pressed && oldms.LeftButton == ButtonState.Released)
                        {
                            if (minicircle.OnClick(tool)) //click
                            {
                                health += 50;
                                clickcircle.Remove(minicircle);
                                break;
                            }
                            else
                            {
                                var instance = sEffect[1].CreateInstance();
                                instance.Volume = 0.5f;
                                instance.Play();
                                health -= 10;
                                clickcircle.Remove(minicircle);
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (ms.LeftButton == ButtonState.Pressed && oldms.LeftButton == ButtonState.Released)
                        {
                            var instance = sEffect[1].CreateInstance();
                            instance.Volume = 0.5f;
                            instance.Play();
                            health -= 5;
                        }
                    }
                    if (minicircle.timedestory(elapsed))
                    {
                        var instance = sEffect[1].CreateInstance();
                        instance.Volume = 0.5f;
                        instance.Play();
                        health -= 10;
                        clickcircle.Remove(minicircle);
                        break;
                    }
                }
            }
            Console.WriteLine(health);

            


        }
        protected void update_end(GameTime gameTime)
        {
            b_reset = new Rectangle(500, 500, 100, 70);
            if (time > highesttime)
            {
                highesttime = (int)time;
                file = new FileStream(filepath, FileMode.Create, FileAccess.Write);
                writer = new BinaryWriter(file);
                writer.Write(highesttime);
                writer.Flush();
                writer.Close();
            }
            if (timebeforeend >= 0)
            {
                timebeforeend -= elapsed;
                minute = rnd.Next(1234, 6543);
                second = rnd.Next(1234, 6543);
            }
            else
            {
                if (time >= 60)
                {
                    minute = (int)(time / 60);
                    second = (int)(time % 60);
                }
                else
                {
                    minute = (int)0;
                    second = (int)time;
                }
            }
            
            

            if (b_reset.Contains(ms.X, ms.Y) && ms.LeftButton == ButtonState.Pressed && oldms.LeftButton == ButtonState.Released)
            {
                Reset();
                screen = ScreenState.menu;
            }
        }

        //Drawwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww

        protected void draw_menu(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Blue);
            _spriteBatch.Draw(play_button, b_play, Color.Red);

            if (minute > 0)
            {
                _spriteBatch.DrawString(overfont,"Highest Time: " + minute + " m " + second + " s", new Vector2(500, 500), Color.White);
            }
            else
            {
                _spriteBatch.DrawString(overfont, "Highest Time: " + second + " s", new Vector2(500, 500), Color.White);
            }
            if (opentutorial)
            {
                _spriteBatch.Draw(tutorial, tutorialbox, Color.Lime);
            }
        }

        protected void draw_gameplay(GameTime gameTime)
        {
            _spriteBatch.DrawString(gameplayfont, Convert.ToString((int)time), Vector2.Zero, Color.Black);
            _spriteBatch.Draw(blockbar, healthbar, Color.Red);
            left_hand.DrawFrame(_spriteBatch,1, new Vector2(0, 600),tool);
            right_hand.DrawFrame(_spriteBatch, 1, new Vector2(1000, 600), tool);
            foreach (ClickBu minicircle in clickcircle)
            {
                minicircle.Draw(_spriteBatch);
            }
        }
        protected void draw_end(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Draw(reset_button, b_reset, Color.Red);
            if(minute > 0)
            {
                _spriteBatch.DrawString(overfont, minute+" m "+ second + " s", new Vector2(500, 500), Color.White);
            }
            else
            {
                _spriteBatch.DrawString(overfont, second + " s", new Vector2(500, 500), Color.White);
            }
            
        }

        protected void Reset()
        {
            opentutorial = false;  //reset
            gameplay_start = false;    //reset
            allow_gameopen = true;
            allow_soundend = true;
            clickcircle.Clear(); //reset
            elapsed = 0;   //reset
            tool = 0;   //reset
            health = 100;   //reset
            time = 0;   //reset
            timeforhealth = 0;   //reset
            timestop = 1;   //reset
            timeforcircle = 0;   //reset
            timecreate = 1;    //reset
            timebeforeend = 2;
            timebeforestart = 4;
            timebeforeover = 3.5f;
            minute = 0;
            second = 0;   //reset
    }
    }
}