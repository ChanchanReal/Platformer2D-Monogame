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
    public class MovingTile : CustomTiles
    {
        public float MovementSpeed = 15f;
        public float Direction = 0;
        public Vector2 MainPosition;
        public Vector2 MovementVector;
        public Vector2 PreviousPosition;
        public MovingTile(Texture2D texture,
            Animation idleAnimation,
            Animation moveAnimation, 
            Vector2 position, 
            SoundEffect soundEffect,
            Level level) : 
            base(texture, 
                idleAnimation, 
                moveAnimation,
                position, 
                soundEffect,
                level)
        {
            MainPosition = position;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, BoundingRectangle, Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            float delta =  (float)gameTime.ElapsedGameTime.TotalSeconds;

            BoundingRectangle = new Rectangle((int)OriginalPosition.X,
                (int)OriginalPosition.Y + (Texture.Height * 2) + 4,
                64, Texture.Height);

            DebugHelper.AddToDebugBound(BoundingRectangle, 555);

            Move(delta);
        }

        protected virtual void Move(float delta)
        {

        }
    }

    public class RightMovingTile : MovingTile
    {
        private float tilesToMove;
        public RightMovingTile(Texture2D texture, Animation idleAnimation, Animation moveAnimation,
            Vector2 position, SoundEffect soundEffect, Level level) :
            base(texture, idleAnimation, moveAnimation, position, soundEffect, level)
        {
             tilesToMove = OriginalPosition.X + (64 * 4);
        }

        protected override void Move(float delta)
        {
            base.Move(delta);

            PreviousPosition = OriginalPosition;
            // main position is the original position of the tile while original position is the real position of current tile
            if (OriginalPosition.X <= MainPosition.X)
            {
                Direction = 1;
            }
            else if (OriginalPosition.X >= tilesToMove )
            {
                Direction = -1; 
            }

            Vector2 posUpdate = new Vector2(OriginalPosition.X + (Direction * MovementSpeed * delta),
                OriginalPosition.Y);

            OriginalPosition = posUpdate;

            MovementVector = OriginalPosition - PreviousPosition;
        }
    }
}
