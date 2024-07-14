using Fallen_Knight.GameAssets.Interface;
using Fallen_Knight.GameAssets.Levels;
using Microsoft.Xna.Framework.Graphics;
using Fallen_Knight.GameAssets.Tiles;
using Microsoft.Xna.Framework;
using System;
using Fallen_Knight.GameAssets.Collisions;
using Fallen_Knight.src.Core;
using Fallen_Knight.GameAssets.Character;


namespace Fallen_Knight.GameAssets.Mobs
{
    public abstract class Enemy : IGameEntity
    {
        private Texture2D texture;
        private Level level;
        private bool onGround;
        private Vector2 velocity;

        private bool isPicking = false;
        private const float gravity = 9.8f;
        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, texture.Height, texture.Height);
            }
        }
        public Vector2 Position
        {
            get { return position; }
        }
        private Vector2 position;

        public Enemy(Texture2D texture, Level level, Vector2 position)
        {
            this.texture = texture;
            this.level = level;
            velocity = Vector2.Zero;
            this.position = position;   
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(texture, BoundingRectangle, Color.White);
        }

        public void Update(GameTime gameTime)
        {
            onGround = false;
            EnforceGravity(gameTime);
            Collision();
            PickUpEnemy();
            Console.WriteLine("Enemy Pisition " + Position);
        }

#if DEBUG
        private void PickUpEnemy()
        {
            if (BoundingRectangle.Contains(InputManager.GetMousePosition()) && InputManager.IsMouseLeftButtonDown() && isPicking)
            {
                isPicking = false;
            }
            else if (BoundingRectangle.Contains(InputManager.GetMousePosition()) && InputManager.IsMouseLeftButtonDown()) 
            {
                isPicking = true;
            }

            if (isPicking)
            {
                position = new Vector2(InputManager.GetMousePosition().X - (BoundingRectangle.Width / 2), InputManager.GetMousePosition().Y  - (BoundingRectangle.Height / 2));
            }
        }
#endif

        float previousBottom;
        public void Collision()
        {
            Rectangle currentBound = BoundingRectangle;

            int leftTile = (int)Math.Floor((float)BoundingRectangle.Left / Tiles.Tile.Size.X);
            int rightTile = (int)Math.Ceiling((float)BoundingRectangle.Right / Tiles.Tile.Size.X);
            int topTile = (int)Math.Floor((float)BoundingRectangle.Top / Tiles.Tile.Size.Y);
            int bottomTile = (int)Math.Ceiling((float)BoundingRectangle.Bottom / Tiles.Tile.Size.Y);

            for (int y = topTile; y < bottomTile; y++)
            {
                for (int x = leftTile; x < rightTile; x++)
                {
                    TileType collision = level.GetCollision(x, y);

                    if (collision == TileType.Platform)
                    {
                        Rectangle tileBound = level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(currentBound, tileBound);
                        float absDepthX = Math.Abs(depth.X);
                        float absDepthY = Math.Abs(depth.Y);

                        if (absDepthY < absDepthX)
                        {
                            if (previousBottom <= tileBound.Top)
                            {
                                onGround = true;
                                position = new Vector2(Position.X, Position.Y + depth.Y);
                                currentBound = BoundingRectangle;
                                velocity.Y = 0;
                            }
                        }
                        else
                        {
                            position = new Vector2(Position.X + depth.X, Position.Y);
                            currentBound = BoundingRectangle;
                        }
                    }
                }
            }

            PlayerCollision();

            previousBottom = BoundingRectangle.Bottom;
        }

        private void PlayerCollision()
        {
            Player player = (Player)level.Player;
            Rectangle playerBound = player.BoundingRectangle;

            if (BoundingRectangle.Intersects(playerBound))
            {
                Vector2 depth = RectangleExtensions.GetIntersectionDepth(BoundingRectangle, playerBound);
                
                if (Math.Abs(depth.X) < Math.Abs(depth.Y))
                {
                    position.X += depth.X;
                }
                else
                {
                    position.Y += depth.Y;
                }
            }
        }

        private void EnforceGravity(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            velocity.Y += gravity;
            position = new Vector2(position.X + (velocity.X * delta), position.Y + (velocity.Y * delta));
        }
    }

    public class RobeEnemy : Enemy
    {
        public RobeEnemy(Texture2D texture, Level level, Vector2 position) : base(texture, level, position)
        {
        }
    }
}
