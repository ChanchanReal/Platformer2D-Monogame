using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.GameAssets.Collisions;
using Fallen_Knight.GameAssets.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallen_Knight.GameAssets.Items
{
    public class BonusItem
    {
        private Vector2 position;
        private Circle itemBound;
        private Texture2D texture;
        private Level level;

        float bounce = 0f;

        public bool IsCollected = false;

        public Vector2 Position
        {
            get { return position + new Vector2(0f, bounce); }
        }

        public BonusItem(Vector2 position, Texture2D texture, Level level)
        {
            this.position = position;
            this.texture = texture;
            itemBound = new Circle(position, texture.Width / 2.0f);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the texture centered on the position
            spriteBatch.Draw(texture, Position, null, Color.AliceBlue, 0f, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
        }

        public void Update(GameTime gameTime, Player player)
        {
            foreach (var hitbox in player.hitbox)
            {
                if (itemBound.Intersecting(hitbox))
                {
                    IsCollected = true;
                }
            }
            itemBound.Center = Position;

            AnimateItem(gameTime);
        }

        public Circle GetItemBound()
        {
            return itemBound;
        }

        public void AnimateItem(GameTime gameTime)
        {
            const float BounceHeight = 0.28f;
            const float BounceRate = 3f;
            const float BounceSync = -0.75f;

            double bounceT = gameTime.TotalGameTime.TotalSeconds * BounceRate + position.X * BounceSync;
            bounce = (float)Math.Sin(bounceT) * BounceHeight * texture.Height;
        }
    }
}