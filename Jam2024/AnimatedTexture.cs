#region File Description
//-----------------------------------------------------------------------------
// AnimatedTexture.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Jam2024
{
    public class AnimatedTexture
    {
        public int framecount;
        public Texture2D myTexture;
        public float TimePerFrame;
        private int Frame;
        public int framerow = 1; // frame row
        private int frame_r; // count frame row 
       private int startframe;
        private int endframe;
        private float TotalElapsed;
        private bool Paused;
        private bool Ended;
        private int Overload;
        public int startrow;
        private bool flip = false;
        private int pauseFrame = -1;
        private int pauseRow = -1;

        public float Rotation, Scale, Depth;
        public Vector2 Origin;
        public AnimatedTexture(Vector2 origin, float rotation, float scale, float depth)
        {
            this.Origin = origin;
            this.Rotation = rotation;
            this.Scale = scale;
            this.Depth = depth;
        }
        
        public void Load(Texture2D asset, int frameCount,int frameRow, int framesPerSec)
        {
            framecount = frameCount;
            framerow = frameRow;
            startframe = 0;
            endframe = (frameCount * framerow)-1;
            myTexture = asset;
            TimePerFrame = (float)1 / framesPerSec;
            Frame = 0;
            frame_r = 0;
            TotalElapsed = 0;
            Paused = false;
            Ended = false;
            Overload = 1;
        }
        public void Load(Texture2D asset, int frameCount, int frameRow, int framesPerSec,int startRow)
        {
            framecount = frameCount;
            framerow = frameRow;
            startframe = 0;
            endframe = (frameCount * framerow) - 1;
            myTexture = asset;
            TimePerFrame = (float)1 / framesPerSec;
            Frame = 0;
            frame_r = 0;
            TotalElapsed = 0;
            Paused = false;
            Ended = false;
            Overload = 2;
            startrow = startRow;
        }
        public void Load(string asset, int frameCount, int frameRow, int framesPerSec, int startRow, ContentManager Content)
        {
            framecount = frameCount;
            framerow = frameRow;
            startframe = 0;
            endframe = (frameCount * framerow) - 1;
            myTexture = Content.Load<Texture2D>(asset);
            TimePerFrame = (float)1 / framesPerSec;
            Frame = 0;
            frame_r = 0;
            TotalElapsed = 0;
            Paused = false;
            Ended = false;
            Overload = 2;
            startrow = startRow;
        }
        // class AnimatedTexture
        public void UpdateFrame(float elapsed)
        {
            if (pauseFrame > -1 && pauseRow > -1)
            {
                
                    frame_r = pauseRow;
                    Frame = pauseFrame;
                    Paused = true;
                    pauseFrame = -1;
                    pauseRow = -1;
                
            }
            if (Paused)
                return;
            TotalElapsed += elapsed;
            if (TotalElapsed > TimePerFrame)
            {
                Frame++;
                if (Frame == framecount)
                {
                    frame_r++;
                    if (Overload == 2)
                    {
                        Ended = true;
                    }
                }
                if (frame_r == framerow)
                {
                    frame_r = 0;
                    if (Overload == 1)
                    {
                        Ended = true;
                    }
                }
                
                // Keep the Frame between 0 and the total frames, minus one.
                Frame = Frame % framecount;
                // check start check end

                TotalElapsed -= TimePerFrame;
            }
        }

        // class AnimatedTexture
        public void DrawFrame(SpriteBatch batch, Vector2 screenPos, bool flip)
        {
            this.flip = flip;
            DrawFrame(batch, Frame, screenPos);
        }
        public void DrawFrame(SpriteBatch batch, Vector2 screenPos)
        {
            DrawFrame(batch, Frame, screenPos);
        }
        public void DrawFrame(SpriteBatch batch, Vector2 screenPos, bool flip, Color color)
        {
            this.flip = flip;
            DrawFrame(batch, Frame, screenPos, color);
        }
        public void DrawFrame(SpriteBatch batch, Vector2 screenPos,int row)
        {
            DrawFrame(batch, Frame, screenPos,row);
        }
        public void DrawFrame(SpriteBatch batch, int frame, Vector2 screenPos, Color color)
        {
            int FrameWidth = myTexture.Width / framecount;
            int FrameHeight = myTexture.Height / framerow;
            Rectangle sourcerect = new Rectangle();
            if (Overload == 1)
            {
                sourcerect = new Rectangle(FrameWidth * frame, FrameHeight * frame_r,
                    FrameWidth, FrameHeight);
            }
            if (Overload == 2)
            {
                sourcerect = new Rectangle(FrameWidth * frame, FrameHeight * (startrow - 1),
                    FrameWidth, FrameHeight);
            }
            if (flip == false)
            {
                batch.Draw(myTexture, screenPos, sourcerect, color,
                    Rotation, Origin, Scale, SpriteEffects.None, Depth);
            }
            else
            {
                batch.Draw(myTexture, screenPos, sourcerect, color,
                    Rotation, Origin, Scale, SpriteEffects.FlipHorizontally, Depth);
            }
        }
        public void DrawFrame(SpriteBatch batch, int frame, Vector2 screenPos)
        {
            int FrameWidth = myTexture.Width / framecount;
            int FrameHeight = myTexture.Height / framerow;
            Rectangle sourcerect = new Rectangle();
            if (Overload == 1)
            {
                sourcerect = new Rectangle(FrameWidth * frame, FrameHeight * frame_r,
                    FrameWidth, FrameHeight);
            }
            if (Overload == 2)
            {
                sourcerect = new Rectangle(FrameWidth * frame, FrameHeight * (startrow-1),
                    FrameWidth, FrameHeight);
            }
            if (flip == false)
            {
                batch.Draw(myTexture, screenPos, sourcerect, Color.White,
                    Rotation, Origin, Scale, SpriteEffects.None, Depth);
            }
            else
            {
                batch.Draw(myTexture, screenPos, sourcerect, Color.White,
                    Rotation, Origin, Scale, SpriteEffects.FlipHorizontally, Depth);
            }
        }
        public void DrawFrame(SpriteBatch batch, int frame, Vector2 screenPos,int row)
        {
            int FrameWidth = myTexture.Width / framecount;
            int FrameHeight = myTexture.Height / framerow;
            startrow = row;
            Rectangle sourcerect = new Rectangle();
            sourcerect = new Rectangle(FrameWidth * frame, FrameHeight * (startrow - 1),
                    FrameWidth, FrameHeight);
            if (flip == false)
            {
                batch.Draw(myTexture, screenPos, sourcerect, Color.White,
                    Rotation, Origin, Scale, SpriteEffects.None, Depth);
            }
            else
            {
                batch.Draw(myTexture, screenPos, sourcerect, Color.White,
                    Rotation, Origin, Scale, SpriteEffects.FlipHorizontally, Depth);
            }
        }
        public bool IsPaused
        {
            get { return Paused; }
        }
        public bool IsEnd
        {
            get { return Ended; }
        }
        public void Reset()
        {
            Frame = 0;
            TotalElapsed = 0f;
            Paused = false;
            Ended = false;
        }
        public void Stop()
        {
            Pause();
        }
        public void Play()
        {
            Paused = false;
        }
        public void Pause()
        {
            Paused = true;
        }
        public void Pause(int frame,int row)
        {
            this.pauseFrame = frame;
            this.pauseRow = row;
            
                
            
        }



        public void change_asset(Texture2D asset)
        {
            myTexture = asset;
        }
    }
}
