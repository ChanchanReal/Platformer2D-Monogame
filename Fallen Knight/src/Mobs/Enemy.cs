using Fallen_Knight.GameAssets.Animations;
using Fallen_Knight.GameAssets.Bots;
using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.GameAssets.Collisions;
using Fallen_Knight.GameAssets.Interface;
using Fallen_Knight.GameAssets.Levels;
using Fallen_Knight.GameAssets.Observer;
using Fallen_Knight.GameAssets.Tiles;
using Fallen_Knight.src.Core;
using Fallen_Knight.src.Interface;
using Fallen_Knight.src.Score;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Fallen_Knight.GameAssets.Mobs
{
    public abstract class Enemy : IGameEntity
    {
        private Bot _ai;
        private Texture2D texture;
        private Vector2 velocity;
        private Level level;
        private FaceDirection faceDirection = FaceDirection.Left;
        private bool onGround;

        private bool isPicking = false;
        private const float gravity = 9.8f;
        private float movementSpeed = 30f;

        private float dieDelay = 0.7f;

        Animation idleAnimation;
        Animation dieAnimation;

        private Vector2 position;
        private bool shouldDie = false;
        private bool isAlive;

        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, 32, 64);
            }
        }
        public Vector2 Position
        {
            get { return position; }
        }
        public bool IsAlive
        {
            get => isAlive;
        }

        public Enemy(Texture2D idleTexture, Texture2D dieTexture, Level level, Vector2 position)
        {
            _ai = new Bot(4f);
            this.texture = idleTexture;
            this.level = level;
            this.position = position;
            velocity = Vector2.Zero;
            idleAnimation = new Animation(idleTexture, 32, 64);
            dieAnimation = new Animation(dieTexture, 64, 64);
            isAlive = true;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!shouldDie)
            {
                idleAnimation.Draw(spriteBatch);
            }
            else
            {
                dieAnimation.Draw(spriteBatch);
            }
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            onGround = false;
            EnforceGravity(gameTime);
            Collision();
            PickUpEnemy();
            SimpleMovement();
            UpdateAI(gameTime);
            UpdateAnimations(gameTime);
            HandleDeath(delta);
        }

        private void UpdateAI(GameTime gameTime)
        {
            if (!shouldDie)
            {
                _ai.Move(ref faceDirection, (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        private void UpdateAnimations(GameTime gameTime)
        {
            if (!shouldDie)
            {
                idleAnimation.Position = BoundingRectangle;
                dieAnimation.Position = new Rectangle(BoundingRectangle.X, BoundingRectangle.Y, 64, 64);
                idleAnimation.UpdateFrame(gameTime);
            }
            else
            {
                dieAnimation.UpdateFrame(gameTime);
            }
        }

        private void HandleDeath(float delta)
        {
            if (shouldDie)
            {
                dieDelay -= delta;
                if (dieDelay < 0)
                {
                    isAlive = false;
                }
            }
        }

        public void SimpleMovement()
        {
            if (faceDirection == FaceDirection.Left)
            {
                velocity.X = -movementSpeed;
                idleAnimation.FlipH = true;
            }
            else
            {
                velocity.X = movementSpeed;
                idleAnimation.FlipH = false;
            }
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
                position = new Vector2(InputManager.GetMousePosition().X - (BoundingRectangle.Width / 2), InputManager.GetMousePosition().Y - (BoundingRectangle.Height / 2));
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
                        if (absDepthX < absDepthY)
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
                    for (int pos = 0; pos < player.Hitbox.Length; pos++)
                    {
                        if (player.Hitbox[pos].Intersects(BoundingRectangle))
                        {
                            //position.X += depth.X;
                        }
                    }

                }
                else
                {
                    if (depth.X > 0)
                    {
                        position.Y += depth.Y;
                    }
                }

                for (int pos = 0; pos < player.Hitbox.Length; pos++)
                {
                    if (player.Hitbox[pos].Intersects(BoundingRectangle))
                    {
                        if (!shouldDie)
                        {
                            player.SetPlayerToDead();
                            isAlive = false;
                        }
                    }
                }
            }
        }

        private void EnforceGravity(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            velocity.Y += gravity;
            position = new Vector2(position.X + (velocity.X * delta), position.Y + (velocity.Y * delta));
        }

        public void KillEnemy()
        {
            shouldDie = true;
            Score.AddScore(5);
            ObserverManager.NotifyCamera();
        }
    }

    public class Executionair : Enemy
    {
        public Executionair(Texture2D texture, Texture2D dieTexture, Level level, Vector2 position)
            : base(texture, dieTexture, level, position)
        {
        }
    }
}
