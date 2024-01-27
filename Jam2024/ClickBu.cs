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
        private List<Texture2D> circlescale,asset;
        private int tool;
        private Vector2 pos;
        private float _timedestroy = 0;
        public Rectangle hitbox,posbox;
        private float scale = 0;

        public ClickBu(List<Texture2D> asset,List<Texture2D> circlescale,int tool, Vector2 pos)
        {
            this.asset = asset;
            this.tool = tool;
            this.pos = pos;
            this.circlescale = circlescale;
        }

        public void Update(float elapsed)
        {
            scale += 0.5f;
            posbox = new Rectangle((int)pos.X, (int)pos.Y, 80, 80);
            hitbox = new Rectangle ((int)(pos.X-((hitbox.Width-80)/2)), (int)(pos.Y-((hitbox.Height-80)/2)), (int)(170-scale), (int)(170-scale));
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(circlescale[tool-1], hitbox, Color.White);
            spriteBatch.Draw(asset[tool-1], posbox, Color.White);
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
