using Fallen_Knight.GameAssets.Animations;
using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.GameAssets.Levels;
using Fallen_Knight.GameAssets.Observer;
using Fallen_Knight.src.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Fallen_Knight.GameAssets.Tiles
{
    public class FallingTile : CustomTiles
    {
        private readonly Random random;
        private readonly SoundEffect gameSound;
        private Vector2 oldPos;

        private float touchDelay = 0.5f;
        private float delayDuration = 0f;
        private float fallSpeed = 250f;
        private bool fall = false;
        private float angle = 0f;
        private float angularVelocity = 0f;

        private int textureWidth = 64;
        private int textureHeight = 20;

        public FallingTile(
            Texture2D texture,
            Animation idleAnimation,
            Animation moveAnimation,
            Vector2 position,
            SoundEffect soundEffect,
            Level level
            ) : base (texture, 
                idleAnimation, 
                moveAnimation, 
                position, 
                soundEffect, 
                level)
        {
            BoundingRectangle = new Rectangle(0, 0, textureWidth, textureHeight);
            oldPos = position;
            random = new Random();
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            spriteBatch.Draw(Texture,
                new Rectangle((int)((OriginalPosition.X + origin.X)),
                (int)(OriginalPosition.Y + origin.Y),
                BoundingRectangle.Width, BoundingRectangle.Height), null, Color.White, 
                angle, origin, SpriteEffects.None, 0f);
        }


        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            BoundingRectangle = new Rectangle((int)OriginalPosition.X, (int)OriginalPosition.Y, 64, 20);

            Player player = Level.Player as Player;

            if (!fall)
            {
                if (BoundingRectangle.Intersects(player.Hitbox[3]))
                {
                    fall = true;
                    delayDuration = touchDelay;
                    ObserverManager.NotifyCamera();
                    angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
                    SoundEffects.Play();
                }
            }

            if (fall)
            {
                if (delayDuration > 0) 
                {
                    delayDuration -= delta;
                }
                else
                {
                    OriginalPosition = new Vector2(OriginalPosition.X, OriginalPosition.Y + fallSpeed * delta);
                    angle += angularVelocity;
                }
            }

            if (OriginalPosition.Y >= 2000)
            {
                OriginalPosition = oldPos;
                angle = 0;
                fall = false;
            }
        }
    }
}
