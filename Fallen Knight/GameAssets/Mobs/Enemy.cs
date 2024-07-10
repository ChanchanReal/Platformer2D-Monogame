using Fallen_Knight.GameAssets.Bots;
using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.GameAssets.Interface;
using Fallen_Knight.GameAssets.Levels;
using Fallen_Knight.GameAssets.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fallen_Knight.GameAssets.Mobs
{
    public class Enemy : IGameEntity
    {
        Texture2D texture;
        Bot enemyBot;
        public Rectangle EnemyBound;
        public float Hp = 100.0f;
        private Vector2 position;
        private bool goingLeft;
        private const float PerMoveDelay = 1f;

        private float gravity = 9.8f;

        private bool isOnGround = false;

        Vector2 velocity = Vector2.Zero;

        public Level Level
        {
            get { return level; }
        }
        private Level level;


        public bool IsAlive
        {
            get
            {
                if (Hp > 0f)
                {
                    return true;
                }

                return false;
            }
        }

        public Vector2 Origin
        {
            get { return new Vector2(texture.Height / 2, texture.Height / 2); }
        }

        public Enemy(Texture2D texture, Vector2 position, Level level)
        {
            this.texture = texture;
            this.position = position;
            this.level = level;
            EnemyBound = new Rectangle((int)position.X, (int)position.Y, 32, 32);
            enemyBot = new Bot(PerMoveDelay);
        }
        public void Update(GameTime gameTime)
        {
            isOnGround = false;

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            AutoMove(delta);
            HandleGravity(delta);
            GetCollision();

            EnemyBound = new Rectangle(
           (int)(position.X - Origin.X),
           (int)(position.Y - Origin.Y),
           texture.Height,
           texture.Height);

        }

        public void AutoMove(float elapse)
        {
            enemyBot.Move(ref goingLeft, elapse);

            if (goingLeft)
            {
                velocity.X = -10f;
            }
            else
            {
                velocity.X = 10f;
            }
        }

        public void HandleGravity(float delta)
        {
            velocity.Y += gravity;
            position = new Vector2(position.X + velocity.X * delta, position.Y + velocity.Y * delta);
        }

        public void GetCollision()
        {
            Rectangle boundingBox = EnemyBound;
            isOnGround = false;

            Player player = (Player)level.Player;

            if (player.Hitbox[1].Intersects(boundingBox))
            {
                goingLeft = true;
                position.X = player.Hitbox[1].X - (player.Hitbox[1].Width - 1);
            }
            else if (player.Hitbox[2].Intersects(boundingBox))
            {
                goingLeft = false;
                position.X = player.Hitbox[2].X + (boundingBox.Width + 1);
            }

            foreach (var tile in Level.tileMap.Keys)
            {
                if (Level.tileMap[tile].Item1 == TileType.Platform)
                {
                    if (boundingBox.Intersects(tile))
                    {
                        Rectangle intersection = Rectangle.Intersect(boundingBox, tile);

                        if (intersection.Width < intersection.Height)
                        {
                            // Horizontal collision
                            if (boundingBox.Right > tile.Left && boundingBox.Left < tile.Left)
                            {
                                position.X = tile.Left - (float)boundingBox.Width;
                                goingLeft = true;
                            }
                            else if (boundingBox.Left < tile.Right && boundingBox.Right > tile.Right)
                            {
                                position.X = tile.Right + (float)(boundingBox.Width * 0.5);
                                goingLeft = false;
                            }
                            velocity.X = 0;
                        }
                        else
                        {
                            // Vertical collision
                            if (boundingBox.Bottom > tile.Top && boundingBox.Top < tile.Top)
                            {
                                position.Y = tile.Y - boundingBox.Height + 12;
                                velocity.Y = 0;
                                isOnGround = true;
                            }
                            else if (boundingBox.Top < tile.Bottom && boundingBox.Bottom > tile.Bottom)
                            {
                                position.Y = tile.Bottom;
                            }
                        }

                        boundingBox.X = (int)position.X;
                        boundingBox.Y = (int)position.Y;
                    }
                }
            }

            EnemyBound = new Rectangle((int)position.X, (int)position.Y, boundingBox.Width, boundingBox.Height);
        }

        public bool CollideWithPlayer(Rectangle rectangle)
        {
            return EnemyBound.Intersects(rectangle);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,
                position,
                new Rectangle(0, 0, texture.Height, texture.Height),
                Color.White,
                0.0f,
                Origin,
                1.0f,
                SpriteEffects.None,
                0);
        }
    }
}
