using Fallen_Knight.GameAssets.Animations;
using Fallen_Knight.GameAssets.Levels;
using Fallen_Knight.src.Core;
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
    public interface ICustomTiles
    {
        public void Update(GameTime gameTime);
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
    public abstract class CustomTiles : ICustomTiles
    {
        protected Texture2D Texture { get; set; }
        public Animation? IdleAnimation { get;}
        public Animation? MoveAnimation { get; }
        public SoundEffect? SoundEffects { get;  }
        public Rectangle BoundingRectangle { get; set; }

        public Vector2 OriginalPosition { get; set; }

        public Level Level { get; }
        public CustomTiles(
            Texture2D texture, 
            Animation idleAnimation,
            Animation moveAnimation,
            Vector2 position,
            SoundEffect soundEffect,
            Level level
            )
        {
            this.Texture = texture;
            IdleAnimation = idleAnimation;
            MoveAnimation = moveAnimation;
            SoundEffects = soundEffect;
            OriginalPosition = position;
            Level = level;
        }

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        public abstract void Update(GameTime gameTime);
    }
}
