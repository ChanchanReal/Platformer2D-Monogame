using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.GameAssets.Interface;
using Fallen_Knight.GameAssets.Levels;
using Fallen_Knight.GameAssets.Observer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Fallen_Knight.GameAssets.Tile.Tile
{
    public class FallingTile : IGameEntity
    {
        Level level;
        Vector2 oldPos;

        public Vector2 Position;
        public Rectangle BoundingRec;
        public Texture2D Texture;

        float touchDelay = 0.5f;
        float timer = 0f;
        float fallSpeed = 10f;
        bool fall = false;

        public FallingTile(Texture2D texture, Vector2 position, Level level)
        {
            this.Texture = texture;
            this.level = level;
            this.Position = position;
            oldPos = position;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, BoundingRec, Color.AliceBlue);
        }

        public void Update(GameTime gameTime)
        {
            BoundingRec = new Rectangle((int)Position.X - 11, (int)Position.Y, 64, 64);

            Player player = level.Player as Player;

            if (!fall)
            {
                if (player.Hitbox[1].Intersects(BoundingRec) 
                    || player.Hitbox[2].Intersects(BoundingRec) 
                    || player.Hitbox[3].Intersects(BoundingRec))
                {
                    fall = true;
                    timer = touchDelay;
                    ObserverManager.NotifyCamera();
                }
            }

            if (fall)
            {
                if (timer > 0) 
                {
                    timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    Position = new Vector2(Position.X, Position.Y + fallSpeed);
                }
            }

            if (Position.Y >= 2000)
            {
                Position = oldPos;
                fall = false;
            }
        }
    }
}
