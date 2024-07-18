using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.GameAssets.Interface;
using Fallen_Knight.GameAssets.Levels;
using Fallen_Knight.GameAssets.Observer;
using Fallen_Knight.src.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Fallen_Knight.GameAssets.Tile.Tile
{
    public class FallingTile : IGameEntity
    {
        Random random;
        Level level;
        Vector2 oldPos;
        public Vector2 Position;
        public Rectangle BoundingRec;
        public Texture2D Texture;
        private readonly GameSoundManager gameSound;

        float touchDelay = 0.5f;
        float delayDuration = 0f;
        float fallSpeed = 250f;
        bool fall = false;
        float angle = 0f;
        float angularVelocity = 0f;

        public FallingTile(Texture2D texture, Vector2 position, Level level , GameSoundManager gameSound)
        {
            this.Texture = texture;
            this.level = level;
            this.Position = position;
            this.gameSound = gameSound;
            oldPos = position;
            random = new Random();
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            spriteBatch.Draw(Texture,
                new Rectangle((int)((Position.X + origin.X) - 10),
                (int)(Position.Y + origin.Y),
                BoundingRec.Width, BoundingRec.Height ), null, Color.White, 
                angle, origin, SpriteEffects.None, 0f);
        }


        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            BoundingRec = new Rectangle((int)Position.X - 11, (int)Position.Y, 64, 64);

            Player player = level.Player as Player;

            if (!fall)
            {
                if (player.Hitbox[1].Intersects(BoundingRec) 
                    || player.Hitbox[2].Intersects(BoundingRec) 
                    || player.Hitbox[3].Intersects(BoundingRec))
                {
                    fall = true;
                    delayDuration = touchDelay;
                    ObserverManager.NotifyCamera();
                    angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
                    gameSound.PlayFallingTileSound(delta);
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
                    Position = new Vector2(Position.X, Position.Y + fallSpeed * delta);
                    angle += angularVelocity;
                }
            }

            if (Position.Y >= 2000)
            {
                Position = oldPos;
                angle = 0;
                fall = false;
            }
        }
    }
}
