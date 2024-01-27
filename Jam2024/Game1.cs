using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SharpDX.Direct2D1.Effects;
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
        private AnimatedTexture acc_bing = new AnimatedTexture(Vector2.Zero, 0, 1, 0);
        private AnimatedTexture acc_meme = new AnimatedTexture(Vector2.Zero, 0, 1, 0);
        private AnimatedTexture domainexpansion = new AnimatedTexture(Vector2.Zero, 0, 1, 0);
        private KeyboardState ks,oldks;
        private MouseState ms, oldms;
        private SpriteBatch _spriteBatch;
        private Texture2D Gameplay_bg, Light, Menu_bg, opacity_;
        private Texture2D play_button, play_circle, reset_button, tutorial, blockbar, guideingame,bar;
        private Texture2D hand_default, hand_banana, hand_eto, hand_scissor, hand_kumpe, hand_glove;
        private Texture2D hand_banana_click, hand_eto_click, hand_scissor_click, hand_kumpe_click;
        private List<Texture2D> circle_effect = new List<Texture2D>();
        private List<Texture2D> icon = new List<Texture2D>();
        private List<Texture2D> patient = new List<Texture2D>();
        private SpriteFont overfont, gameplayfont;
        bool opentutorial = false;  //reset
        private ScreenState screen;
        private HandState hand;
        private Random rnd = new Random();
        private bool lighton = false; //reset
        private bool gameplay_start = false, allow_gameopen = true, allow_soundend = true;    //reset
        private List<ClickBu> clickcircle = new List<ClickBu>();    //reset
        private float elapsed = 0;   //reset
        private float scale = 0, scalespeed = 1;
        private int tool = 0;   //reset

        private Vector2 hand_position = new Vector2(700,330);
        private Rectangle b_play, b_play_circle, b_reset, tutorialbox, healthbar, removehealthbar;

        private float max_health = 100;
        private float health = 100;   //reset
        private float time = 0;   //reset
        private float timeforhealth = 0;   //reset
        private float timestop = 1;   //reset
        private float timeforcircle = 0;   //reset
        private float timecreate = 1;    //reset
        private float timebeforeend = 2; //reset
        private float timebeforestart = 5; //reset
        private float timebeforeover = 3.5f;

        private int highesttime = 0;
        private int score = 0;

        private string filepath;
        private FileStream file;
        private BinaryReader reader;
        private BinaryWriter writer;

        static public List<SoundEffect> sEffect = new List<SoundEffect>();
        private int main_volume_effect = 1;

        private Song OnMenu, OnGamePlay, OnEnd;
        ClickState clicktrue = ClickState.none;
        enum ClickState
        {
            yes,
            no,
            none
        }
        enum HandState
        {
            normal,
            banana,
            banana_click,
            eto,
            eto_click,
            scissor,
            scissor_click,
            kumpe,
            kumpe_click,
            DomainExpansion
        }
        enum ScreenState
        {
            menu,
            gameplay,
            end
        }

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
            hand = HandState.normal;
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
            circle_effect.Add(Content.Load<Texture2D>("Circle/circle_effect_eto"));
            circle_effect.Add(Content.Load<Texture2D>("Circle/circle_effect_banana"));
            circle_effect.Add(Content.Load<Texture2D>("Circle/circle_effect_scissor"));
            circle_effect.Add(Content.Load<Texture2D>("Circle/circle_effect_kumpe"));
            icon.Add(Content.Load<Texture2D>("UI/icon_eto"));
            icon.Add(Content.Load<Texture2D>("UI/icon_banana"));
            icon.Add(Content.Load<Texture2D>("UI/icon_scissor"));
            icon.Add(Content.Load<Texture2D>("UI/icon_kumpe"));
            opacity_ = Content.Load<Texture2D>("Background/dark");
            Gameplay_bg = Content.Load<Texture2D>("Background/Gameplay_background");
            Menu_bg = Content.Load<Texture2D>("Background/Menu_background");
            Light = Content.Load<Texture2D>("Background/Light");
            hand_default = Content.Load<Texture2D>("Hand/hand_default");
            hand_glove = Content.Load<Texture2D>("Hand/hand_glove");
            hand_banana = Content.Load<Texture2D>("Hand/hand_banana");
            hand_banana_click = Content.Load<Texture2D>("Hand/hand_banana_click");
            hand_eto = Content.Load<Texture2D>("Hand/hand_eto");
            hand_eto_click = Content.Load<Texture2D>("Hand/hand_eto_click");
            hand_scissor = Content.Load<Texture2D>("Hand/hand_scissor");
            hand_scissor_click = Content.Load<Texture2D>("Hand/hand_scissor_click");
            hand_kumpe = Content.Load<Texture2D>("Hand/hand_kumpe");
            hand_kumpe_click = Content.Load<Texture2D>("Hand/hand_kumpe_click");
            patient.Add(Content.Load<Texture2D>("Background/patient/patient_normal"));
            patient.Add(Content.Load<Texture2D>("Background/patient/patient_eto"));
            patient.Add(Content.Load<Texture2D>("Background/patient/patient_banana"));
            patient.Add(Content.Load<Texture2D>("Background/patient/patient_scissor"));
            patient.Add(Content.Load<Texture2D>("Background/patient/patient_kumpe"));
            patient.Add(Content.Load<Texture2D>("Background/patient/patient_death"));
            patient.Add(Content.Load<Texture2D>("Background/patient/patient_hurt"));
            domainexpansion.Load("Hand/hand_expansion", 4, 1, 8, 1, Content);
            acc_bing.Load("Background/acc/bing", 5, 1, 2, 1, Content);
            acc_meme.Load("Background/acc/meme", 5, 1, 1, 1, Content);
            play_button = Content.Load<Texture2D>("UI/playbutton");
            play_circle = Content.Load<Texture2D>("UI/playCircle");
            reset_button = Content.Load<Texture2D>("testtexture");
            tutorial = Content.Load<Texture2D>("testtexture");
            blockbar = Content.Load<Texture2D>("testtexture");
            bar = Content.Load<Texture2D>("UI/bar");
            guideingame = Content.Load<Texture2D>("UI/guideingame");
            overfont = Content.Load<SpriteFont>("sfont");
            gameplayfont = Content.Load<SpriteFont>("gameplayfont");
            sEffect.Add(Content.Load<SoundEffect>("Sound/OpenGamePlay"));     //0
            sEffect.Add(Content.Load<SoundEffect>("Sound/hurt"));     //1
            sEffect.Add(Content.Load<SoundEffect>("Sound/hurt_itai"));     //1
            sEffect.Add(Content.Load<SoundEffect>("Sound/hurt_jeb"));     //1
            sEffect.Add(Content.Load<SoundEffect>("Sound/death"));     //2
            sEffect.Add(Content.Load<SoundEffect>("Sound/eto"));     //3
            sEffect.Add(Content.Load<SoundEffect>("Sound/banana"));     //4
            sEffect.Add(Content.Load<SoundEffect>("Sound/gungai"));     //5
            sEffect.Add(Content.Load<SoundEffect>("Sound/kumpe"));     //6
            SoundEffect.MasterVolume = main_volume_effect;
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
                b_play = new Rectangle(510, 400, 209, 214);
                if (b_play.Contains(ms.X, ms.Y) && ms.LeftButton == ButtonState.Pressed && oldms.LeftButton == ButtonState.Released)
                {
                    opentutorial = true;
                }
            }
            else
            {
                b_play = new Rectangle(510, 500, 209, 214);
                if (b_play.Contains(ms.X, ms.Y) && ms.LeftButton == ButtonState.Pressed && oldms.LeftButton == ButtonState.Released)
                {
                    screen = ScreenState.gameplay;
                }
            }
            b_play_circle = new Rectangle((int)(b_play.X - ((b_play_circle.Width - b_play.Width) / 2)), (int)(b_play.Y - ((b_play_circle.Height - b_play.Height) / 2)), (int)(275 - scale), (int)(275 - scale));
            scale += scalespeed;
            if (scale >= 90 || scale <= 0)
            {
                scalespeed *= -1;
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
                hand = HandState.DomainExpansion;
                if(hand == HandState.DomainExpansion)
                {
                    hand_position.X -= 1;
                    if(hand_position.X <= 530)
                    {
                        hand_position.X = 530;
                        domainexpansion.UpdateFrame(elapsed);
                    }
                }
                timebeforestart -= elapsed;
            }
            else
            {
                if(hand == HandState.DomainExpansion)
                {
                    hand = HandState.normal;
                }
                gameplay_start = true;
            }

            if (health <= 0)
            {
                hand = HandState.normal;
                gameplay_start = false;
                if (allow_soundend)
                {
                    var instance = sEffect[4].CreateInstance();
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
            
            healthbar = new Rectangle(289, 14, (int)(356*(health/max_health)), 21);
            removehealthbar = new Rectangle(289+healthbar.Width, 14, 356-healthbar.Width, 21);
            if (gameplay_start)
            {
                timeforhealth += (float)gameTime.ElapsedGameTime.TotalSeconds;
                timeforcircle += (float)gameTime.ElapsedGameTime.TotalSeconds;
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                timestop = 1 - (time / 90);
                timecreate = 2 - (time / 45);
                acc_bing.UpdateFrame(elapsed);
                acc_meme.UpdateFrame(elapsed);
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
                    clickcircle.Add(new ClickBu(icon, circle_effect, rnd.Next(1, 5), new Vector2(rnd.Next(50, 1100), rnd.Next(50, 400))));
                    timeforcircle = 0;
                }
                //change tool
                if (ks.IsKeyDown(Keys.D1) && oldks.IsKeyUp(Keys.D1))
                {
                    tool = 1;
                }
                else if (ks.IsKeyDown(Keys.D2) && oldks.IsKeyUp(Keys.D2))
                {
                    tool = 2;

                }
                else if (ks.IsKeyDown(Keys.D3) && oldks.IsKeyUp(Keys.D3))
                {
                    tool = 3;

                }
                else if (ks.IsKeyDown(Keys.D4) && oldks.IsKeyUp(Keys.D4))
                {
                    tool = 4;

                }

                if (ms.LeftButton == ButtonState.Released)
                {
                    clicktrue = ClickState.none;
                    if (tool == 0)
                    {
                        hand = HandState.normal;
                    }
                    else if (tool == 1)
                    {
                        hand = HandState.eto;
                    }
                    else if (tool == 2)
                    {
                        hand = HandState.banana;
                    }
                    else if (tool == 3)
                    {
                        hand = HandState.scissor;
                    }
                    else if (tool == 4)
                    {
                        hand = HandState.kumpe;
                    }
                }
                foreach (ClickBu minicircle in clickcircle) //button show
                {
                    
                    minicircle.Update(elapsed);
                    if (minicircle.hitbox.Contains(ms.X, ms.Y))
                    {
                        if (ms.LeftButton == ButtonState.Pressed && oldms.LeftButton == ButtonState.Released)
                        {
                            
                            //sound & animate onclick
                            if (tool == 1)
                            {
                                var instance = sEffect[5].CreateInstance();
                                instance.Volume = 0.5f;
                                instance.Play();
                                hand = HandState.eto_click;
                            }
                            else if (tool == 2)
                            {
                                    var instance = sEffect[6].CreateInstance();
                                    instance.Volume = 0.5f;
                                    instance.Play();
                                    hand = HandState.banana_click;
                            }
                            else if (tool == 3)
                            {
                                var instance = sEffect[7].CreateInstance();
                                instance.Volume = 1f;
                                instance.Play();
                                hand = HandState.scissor_click;
                            }
                            else if (tool == 4)
                            {
                                var instance = sEffect[8].CreateInstance();
                                instance.Volume = 0.5f;
                                instance.Play();
                                hand = HandState.kumpe_click;
                            }
                            

                            if (minicircle.OnClick(tool)) //click
                            {
                                health += 50;
                                clickcircle.Remove(minicircle);
                                clicktrue = ClickState.yes;
                                break;
                            }
                            else
                            {
                                clicktrue = ClickState.no;
                                var instance = sEffect[rnd.Next(1, 4)].CreateInstance();
                                instance.Volume = 0.5f;
                                instance.Play();
                                health -= 10;
                                clickcircle.Remove(minicircle);
                                break;
                            }
                        }
                    }
                    
                    if (minicircle.timedestory(elapsed))
                    {
                        clicktrue = ClickState.no;
                        var instance = sEffect[rnd.Next(1, 4)].CreateInstance();
                        instance.Volume = 0.5f;
                        instance.Play();
                        health -= 10;
                        clickcircle.Remove(minicircle);
                        break;
                    }
                }
            }
            Console.WriteLine(clicktrue);

            


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

                score = rnd.Next(1234, 9876);
            }
            else
            {
                score = (int)time;
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
            _spriteBatch.Draw(Menu_bg, Vector2.Zero, Color.DarkSlateGray);
            if (!opentutorial)
            {
                _spriteBatch.Draw(Light, Vector2.Zero, Color.White);
                _spriteBatch.DrawString(overfont, "Highest Score: " + highesttime, new Vector2(485, 700), Color.White);
            }
            else
            {
                _spriteBatch.Draw(opacity_, Vector2.Zero, Color.White);
            }
            _spriteBatch.Draw(hand_glove, new Vector2(0,5), Color.White);
            _spriteBatch.Draw(play_circle, b_play_circle, Color.White);
            if (b_play.Contains(ms.X, ms.Y))
            {
                _spriteBatch.Draw(play_button, b_play, Color.Gray);
            }
            else
            {
                _spriteBatch.Draw(play_button, b_play, Color.White);
            }

            
           
            
            if (opentutorial)
            {
                _spriteBatch.Draw(tutorial, tutorialbox, Color.Lime);
            }


        }

        protected void draw_gameplay(GameTime gameTime)
        {
            
            if (lighton)
            {
                _spriteBatch.Draw(Gameplay_bg, Vector2.Zero, Color.White);
                acc_bing.DrawFrame(_spriteBatch, new Vector2(342, 27));
                acc_meme.DrawFrame(_spriteBatch, new Vector2(350,179));
                _spriteBatch.Draw(Light, Vector2.Zero, Color.White);

                if (ms.LeftButton == ButtonState.Pressed && health > 0 && clicktrue == ClickState.yes)
                {
                    _spriteBatch.Draw(patient[tool], Vector2.Zero, Color.White);
                }
                else if(ms.LeftButton == ButtonState.Pressed && health > 0 && clicktrue == ClickState.no)
                {
                    _spriteBatch.Draw(patient[6], Vector2.Zero, Color.White);
                }
                else
                {
                    _spriteBatch.Draw(patient[0], Vector2.Zero, Color.White);
                }
                if(health <= 0)
                {
                    _spriteBatch.Draw(patient[5], Vector2.Zero, Color.White);
                }
            }
            else
            {
                _spriteBatch.Draw(Menu_bg, Vector2.Zero, Color.DarkSlateGray);
            }
            _spriteBatch.Draw(blockbar, healthbar, Color.Red);
            _spriteBatch.Draw(blockbar, removehealthbar, Color.White);
            _spriteBatch.Draw(bar, Vector2.Zero, Color.White);
            _spriteBatch.Draw(opacity_, Vector2.Zero, Color.White);
            _spriteBatch.DrawString(gameplayfont, Convert.ToString((int)time), new Vector2(110,20), Color.White);
            

            switch (hand)
            {
                case HandState.normal:
                    _spriteBatch.Draw(hand_default,Vector2.Zero,Color.White); break;
                case HandState.eto:
                    _spriteBatch.Draw(hand_eto, Vector2.Zero, Color.White); break;
                case HandState.eto_click:
                    _spriteBatch.Draw(hand_eto_click, Vector2.Zero, Color.White); break;
                case HandState.banana:
                    _spriteBatch.Draw(hand_banana, Vector2.Zero, Color.White); break;
                case HandState.banana_click:
                    _spriteBatch.Draw(hand_banana_click, Vector2.Zero, Color.White); break;
                case HandState.scissor:
                    _spriteBatch.Draw(hand_scissor, Vector2.Zero, Color.White); break;
                case HandState.scissor_click:
                    _spriteBatch.Draw(hand_scissor_click, Vector2.Zero, Color.White); break;
                case HandState.kumpe:
                    _spriteBatch.Draw(hand_kumpe, Vector2.Zero, Color.White); break;
                case HandState.kumpe_click:
                    _spriteBatch.Draw(hand_kumpe_click, Vector2.Zero, Color.White); break;
                case HandState.DomainExpansion:
                    if (domainexpansion.IsEnd)
                    {
                        domainexpansion.DrawFrame(_spriteBatch, 3, hand_position);
                        lighton = true;
                    }
                    else
                    {
                        domainexpansion.DrawFrame(_spriteBatch, hand_position);
                    }
                    break;
            }

            if (lighton)
            {
                _spriteBatch.Draw(guideingame, new Vector2(455, 650), Color.White);
            }
            foreach (ClickBu minicircle in clickcircle)
            {
                minicircle.Draw(_spriteBatch);
            }
        }
        protected void draw_end(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Draw(reset_button, b_reset, Color.Red);
            _spriteBatch.DrawString(overfont, "Score: "+score, new Vector2(500, 500), Color.White);
            
            
        }

        protected void Reset()
        {
            opentutorial = false;  //reset
            gameplay_start = false;    //reset
            allow_gameopen = true;
            allow_soundend = true;
            lighton = false;
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
            timebeforestart = 5;
            timebeforeover = 3.5f;
            score = 0;
            scale = 0;
            scalespeed = 1;
            hand = HandState.normal;
            domainexpansion.Reset();
            hand_position = new Vector2(700, 330);
            clicktrue = ClickState.none;
        }
    }
}