using Fallen_Knight.GameAssets.Animations;
using Fallen_Knight.GameAssets.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallen_Knight.src.Tiles
{
    public class SpikeTile : CustomTiles
    {
        public SpikeTile(
            Texture2D texture, 
            Animation idleAnimation, 
            Animation moveAnimation,
            Vector2 position,
            SoundEffect soundEffect,
            Level level
            )
            : base(
                  texture,
                  idleAnimation,
                  moveAnimation,
                  position,
                  soundEffect,
                  level
                  )
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, BoundingRectangle, Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            BoundingRectangle = new Rectangle((int)OriginalPosition.X, (int)OriginalPosition.Y + (Texture.Height / 2), 64, Texture.Height / 2);
            DebugHelper.AddToDebugBound(BoundingRectangle, 33);
        }
    }
}
