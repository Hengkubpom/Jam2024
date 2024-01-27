using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Jam2024
{
    
    public class ClickBu
    {
        private AnimatedTexture anim = new AnimatedTexture(Vector2.Zero, 0, 1, 0);
        private Texture2D circlescale;
        private int tool;
        private Vector2 pos;
        private float _timedestroy = 0;
        public Rectangle hitbox;
        private float scale = 0;

        public ClickBu(Texture2D asset,Texture2D circlescale,int tool, Vector2 pos)
        {
            anim.Load(asset, 1, 1, 1);
            this.tool = tool;
            this.pos = pos;
            this.circlescale = circlescale;
        }

        public void Update(float elapsed)
        {
            scale += 0.5f;
            if(scale > 86)
            {
                scale = 86;
            }
            hitbox = new Rectangle ((int)(pos.X-((hitbox.Width-64)/2)), (int)(pos.Y-((hitbox.Height-64)/2)), (int)(150-scale), (int)(150-scale));
            anim.UpdateFrame(elapsed);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(circlescale, hitbox, Color.White);
            if(tool == 1)
            {
                anim.DrawFrame(spriteBatch, 1, pos, Color.Green);
            }
            if (tool == 2)
            {
                anim.DrawFrame(spriteBatch, 1, pos, Color.Red);
            }
            if (tool == 3)
            {
                anim.DrawFrame(spriteBatch, 1, pos, Color.Blue);
            }
            if (tool == 4)
            {
                anim.DrawFrame(spriteBatch, 1, pos, Color.Pink);
            }
        }
        public bool OnClick(int playertool)
        {
            if(playertool == tool)
            {
                return true;
            }

            return false;
        }

        public bool timedestory(float elapsed)
        {
            _timedestroy += elapsed;
            if(_timedestroy > 4)
            {
                return true;
            }
            return false;
        }
    }
}
