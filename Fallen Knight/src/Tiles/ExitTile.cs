using Fallen_Knight.GameAssets.Animations;
using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.GameAssets.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallen_Knight.src.Tiles
{
    public class ExitTile
    {
        private Animation animation;
        private readonly Level level;
        private Texture2D texture;
        private Rectangle boundingRec;

        public ExitTile(Level level, Texture2D texture, Rectangle position)
        {
            this.level = level;
            this.texture = texture;
            this.boundingRec = position;
            animation = new Animation(texture, 64, 64);
            animation.Position = boundingRec;
        }
        public void Update(GameTime gameTime)
        {
            Player player =  (Player)level.Player;
            
            for (int i = 0; i < player.Hitbox.Length; i++)
            {
                if (player.Hitbox[i].Intersects(boundingRec))
                {
                    level.ExitReached = true;
                }
            }
            animation.UpdateFrame(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            animation.Draw(spriteBatch);
        }
    }
}
