using Fallen_Knight.GameAssets.Animations;
using Fallen_Knight.GameAssets.Collisions;
using Fallen_Knight.GameAssets.Interface;
using Fallen_Knight.GameAssets.Levels;
using Fallen_Knight.GameAssets.Tiles;
using Fallen_Knight.src.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Fallen_Knight.GameAssets.Character
{
    public class Player : IGameEntity
    {
        // Display related stuff
        private Animation Idle;
        private Animation Run;
        private Animation Attack;
        private Animation Jump;
        private Animation Fall;
        private Texture2D IdleTexture;
        private Texture2D RunTexture;
        private Texture2D AttackTexture;
        private Texture2D JumpTexture;
        private ContentManager contentManager;
        private ParticleSystem particleSystem;

        // Game physics
        private const float maxWalkingSpeed = 5f;
        private Vector2 playerSpeed = new Vector2(0f, 0f);
        private static float accel = 0.1f;
        private static float friction = accel * 3f;
        private static float tolerance = friction * 0.9f;
        private const float RunSpeed = 300f;
        private const float AttackDelay = 0.4f;

        // jump related physics
        private float jumpSpeed = -8.5f;
        private bool isGround = false;

        // gravity
        private static float gravity = 9.8f / 40;
        Vector2 forces = new Vector2(friction, gravity);

        // Player relation checks
        public PlayerStatus CurrentAction;
        private float attackTimer = 0f;
        private float wantsToJumpDuration = 0f;
        private float cayoteTime = 0f;
        private float movement = 0;
        private bool spriteDirection = false;
        private bool isJumping = false;
        private bool wantsToJump = false;
        private bool headIsColliding = false;
        private int collisionDirection = 0;

        // Hit box for side i don't know
        private const int Head = 0;
        private const int LeftBody = 1;
        private const int RightBody = 2;
        private const int Feet = 3;
        public Rectangle[] Hitbox;

        // for debugging delete this later.
        public float DeltaTime
        {
            get 
            {
                return deltaTime;
            }
        }
        private float deltaTime;
        public Vector2 SpawnPoint
        {
            get => spawnArea;
        }
        private Vector2 spawnArea;

        public bool IsAlive
        {
            get => isAlive;
        }
        private bool isAlive;
        public Level Level
        {
            get => level;
        }
        Level level;
        public Rectangle BoundingRectangle
        {
            get
            {
                // get origin 
                int left = (int)Math.Round(Position.X + (int)0.2f * Idle.FrameWidth);
                int top = (int)Math.Round(Position.Y + (int)0.2f * Idle.FrameHeight);

                return new Rectangle(left, top, PlayerDefaultWidth, SpriteHeight);
            }
        }
        Rectangle boundRectangle;

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }
        Vector2 position;

        public Player(Level level, ContentManager contentManager)
        {
            this.level = level;
            this.contentManager = contentManager;
            isAlive = true;
            LoadContent();
        }

        private const int PlayerDefaultWidth = 64;
        private const int PlayerAttackingWidth = 125;
        private const int SpriteHeight = 86;

        public void LoadContent()
        {
            LoadTexture();
            LoadAnimation();
            Position = SpawnPoint;
            isAlive = true;

            int width = (int)(Idle.FrameWidth * 0.4);
            int left = (Idle.FrameWidth - width) / 2;
            int height = (int)(Idle.FrameHeight * 0.8);
            int top = Idle.FrameHeight - height;

            boundRectangle = new Rectangle((int)Position.X - PlayerAttackingWidth, (int)Position.Y, PlayerDefaultWidth, SpriteHeight);
            List<Texture2D> texture = new List<Texture2D>();
            texture.Add(contentManager.Load<Texture2D>("diamond"));
            particleSystem = new ParticleSystem(Position, texture);
            LoadHitBox();
        }
        private void LoadTexture()
        {
            IdleTexture = contentManager.Load<Texture2D>("Player/Idle");
            RunTexture = contentManager.Load<Texture2D>("Player/Run");
            AttackTexture = contentManager.Load<Texture2D>("Player/Attack 1");
        }

        private void LoadAnimation()
        {
            Run = new Animation(RunTexture, 72, SpriteHeight);
            Fall = new Animation(contentManager.Load<Texture2D>("Player/Fall-Only"), 80, 86);
            Jump = new Animation(contentManager.Load<Texture2D>("Player/Jump-Only"), 80, 86);
            Idle = new Animation(IdleTexture, PlayerDefaultWidth, SpriteHeight);
            Attack = new Animation(AttackTexture, PlayerAttackingWidth, SpriteHeight);
        }
        /// <summary>
        /// Do adjustment necessary to match hit box
        /// very annoying to change
        /// </summary>
        private void LoadHitBox()
        {
            if (Hitbox == null)
            {
                Hitbox = new Rectangle[4];
            }

            int topHitboxWidth = (int)(BoundingRectangle.Width * 0.2f);
            int topHitboxHeight = (int)(BoundingRectangle.Height * 0.3f);
            int topHitboxX = BoundingRectangle.X + (BoundingRectangle.Width - topHitboxWidth) / 2;
            int topHitboxY = BoundingRectangle.Y + (int)(BoundingRectangle.Height * 0.2f);

            Hitbox[Head] = new Rectangle(topHitboxX, topHitboxY, topHitboxWidth, topHitboxHeight);

            int leftBodyX = BoundingRectangle.X + topHitboxWidth;
            int leftBodyY = topHitboxY + topHitboxHeight;
            int leftBodyWidth = topHitboxWidth;
            int leftBodyHeight = topHitboxHeight;

            Hitbox[LeftBody] = new Rectangle(leftBodyX, leftBodyY, leftBodyWidth, leftBodyHeight);

            int rightBodyX = topHitboxX + topHitboxWidth;
            int rightBodyY = leftBodyY;
            int rightBodyWidth = leftBodyWidth;
            int rightBodyHeight = leftBodyHeight;

            Hitbox[RightBody] = new Rectangle(rightBodyX, rightBodyY, rightBodyWidth, rightBodyHeight);

            int feetWidth = (int)(BoundingRectangle.Width * 0.3f);
            int feetHeight = (int)(BoundingRectangle.Height * 0.3f);
            int feetX = BoundingRectangle.X + (BoundingRectangle.Width - feetWidth) / 2;
            int feetY = rightBodyY + (rightBodyHeight + feetHeight) / 2;

            int updateFeetY = feetY - (int)(feetY * 0.01f);

            Hitbox[Feet] = new Rectangle(feetX, updateFeetY, feetWidth, feetHeight);
        }
        public void Update(GameTime gameTime)
        {
            SetDeltaTime(gameTime);
            movement = 0;

            if (CurrentAction == PlayerStatus.Attacking)
            {
                attackTimer -= DeltaTime;
                if (attackTimer <= 0)
                {
                    CurrentAction = PlayerStatus.Idle;
                    attackTimer = 0;
                }
            }

            GetInput(gameTime);
            EnforceGravity(gameTime);
            SwitchCurrentActionJump();
            IsCharacterDead();
            LoadHitBox();
            particleSystem.Update(this);
        }

        public void IsCharacterDead()
        {
            if (!IsAlive)
            {
                Position = spawnArea;
                playerSpeed = Vector2.Zero;
                isAlive = true;
            }
        }
        public void SwitchCurrentActionJump()
        {
            if (playerSpeed.Y < 0)
            {
                CurrentAction = PlayerStatus.Jump;
            }
            else if (playerSpeed.Y > 0)
            {
                CurrentAction = PlayerStatus.Falling;
            }
        }
        private void GetInput(GameTime gameTime)
        {
            ICommand input = HandleInput(gameTime);

            if (input != null)
            {
                playerSpeed = input.Execute(gameTime);
            }

            if (InputManager.Input(Keys.F))
            {
                CurrentAction = PlayerStatus.Attacking;
                attackTimer = AttackDelay;
            }

            if (CurrentAction == PlayerStatus.Attacking)
            {
                Attack.UpdateFrame(gameTime);
            }
        }

        private void SetDeltaTime(GameTime gameTime)
        {
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        public ICommand HandleInput(GameTime gameTime)
        {
            if (!isGround && InputManager.Input(Keys.Space))
            {
                wantsToJumpDuration = 0.2f;
                wantsToJump = true;
            }

            if (InputManager.Input(Keys.K))
            {
                Position = SpawnPoint;
            }

            if (wantsToJump && wantsToJumpDuration <= 0)
            {
                wantsToJump = false;
            }

            if (isGround && !headIsColliding && (wantsToJump ||
                (InputManager.Input(Keys.Space))))
            {
                isGround = false;
                isJumping = true;
                wantsToJump = false;
                wantsToJumpDuration = 0f;
                CurrentAction = PlayerStatus.Jump;
                Jump.FlipH = spriteDirection;
                Fall.FlipH = Jump.FlipH;
                return new Jump(maxWalkingSpeed, accel, jumpSpeed, playerSpeed, Jump);
            }

            // Decrement wantsToJumpDuration regardless of whether a jump occurred
            wantsToJumpDuration -= DeltaTime;

            if (InputManager.Input(Keys.D) && isGround && collisionDirection != 1)
            {
                spriteDirection = false;
                CurrentAction = PlayerStatus.Walking;
                return new Dash(maxWalkingSpeed, accel, jumpSpeed, playerSpeed, Run);
            }

            if (InputManager.HoldableInput(Keys.Right) && collisionDirection != 1)
            {
                spriteDirection = false;
                CurrentAction = PlayerStatus.Walking;
                GenerateParticles(gameTime);
                return new MoveRight(maxWalkingSpeed, accel, jumpSpeed, playerSpeed, Run);
            }
            else if (InputManager.HoldableInput(Keys.Left) && collisionDirection != -1)
            {
                spriteDirection = true;
                CurrentAction = PlayerStatus.Walking;
                GenerateParticles(gameTime);
                return new MoveLeft(maxWalkingSpeed, accel, jumpSpeed, playerSpeed, Run);
            }
            else
            {
                ApplyFriction();

                if (playerSpeed.X == 0 && collisionDirection == 0)
                {
                    Idle.FlipH = spriteDirection;
                    CurrentAction = PlayerStatus.Idle;
                    Idle.UpdateFrame(gameTime);
                }
            }
            return null;
        }
        private void ApplyFriction()
        {
            if (isGround)
            {
                playerSpeed.X += -Math.Sign(playerSpeed.X) * forces.X;

                if (Math.Abs(playerSpeed.X) <= tolerance)
                {
                    playerSpeed.X = 0;
                }
            }
        }
        public void GenerateParticles(GameTime gameTime)
        {
            float elapse = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!isGround)
                return;

            if (particleSystem.DelayTime >= 0)
            {
                particleSystem.DelayTime -= elapse;
            }
            else
            {
                particleSystem.DelayTime = 0.1f;
                if (playerSpeed.X != 0)
                particleSystem.StartGeneratingParticle();
            }
        }
        public void Draw(SpriteBatch sprite)
        {
                switch (CurrentAction)
                {
                    case PlayerStatus.Idle:
                        Idle.Draw(sprite, BoundingRectangle);
                        break;

                    case PlayerStatus.Walking:
                        Run.Draw(sprite, BoundingRectangle);
                        break;

                    case PlayerStatus.Attacking:
                        Attack.Draw(sprite,
                            new(BoundingRectangle.X,
                            BoundingRectangle.Y,
                            PlayerAttackingWidth,
                            SpriteHeight));
                        break;

                    case PlayerStatus.Jump:
                        Jump.Draw(sprite, BoundingRectangle);
                        break;

                    case PlayerStatus.Falling:
                        Fall.Draw(sprite, BoundingRectangle);
                        break;
                }

            particleSystem.Draw(sprite);
        }
        private float previousBottom;
        private float previousLeft;
        private float previousPos;
        private void GetCollision(Rectangle newBoundRectangle, Vector2 velocity)
        {
            Rectangle bounds = newBoundRectangle;

            if (cayoteTime <= 0)
                isGround = false;

            collisionDirection = 0;

            // gets the player bounds in game world
            int leftTile = (int)Math.Floor((float)Hitbox[LeftBody].Left / 64);
            int rightTile = (int)Math.Ceiling(((float)Hitbox[RightBody].Right / 64)) - 1;
            int topTile = (int)Math.Floor((float)Hitbox[Head].Top / 65);
            int bottomTile = (int)Math.Ceiling(((float)Hitbox[Feet].Bottom / 65)) - 1;

            // checks for the closest tile between the player
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    if (level.GetCollision(x, y) == TileType.Platform)
                    {
                        // gets the tile bound and depth of how player and tile intersects
                        Rectangle tileBound = Level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBound);

                        if (depth == Vector2.Zero)
                            return;

                        // check Height is less than width depth
                        if (Math.Abs(depth.Y) < Math.Abs(depth.X))
                        {
                            // if we our on top of tile.
                            if (previousBottom <= tileBound.Top && playerSpeed.Y >= 0 && Hitbox[Feet].Intersects(tileBound))
                            {
                                isGround = true;
                                playerSpeed.Y = 0;
                                Position = new Vector2(Position.X, tileBound.Top - bounds.Height + 1);
                            }
                            else if (playerSpeed.Y < 0 && Hitbox[Head].Intersects(tileBound)) // if we hit the top of the tile as well as our velocity is jumping
                            {
                                // Hitting head on the ceiling
                                Position = new Vector2(Position.X, tileBound.Y + tileBound.Height * 0.8f);
                                playerSpeed.Y = 0; // Stop upward movement
                            }
                            // update the bound of the player to re-check the neighboring tile
                            bounds = BoundingRectangle;
                        }
                        // Resolve horizontal collision
                        else
                        {
                            // making sure we collided a wall collision tile.
                            if (depth.X != 0)
                            {
                                // if left wall as well as double checking to makesure we did intersect with our custom hitbox
                                if (depth.X > 0)
                                {
                                    if (Hitbox[LeftBody].Intersects(tileBound))
                                    Position = new Vector2(tileBound.X + (bounds.Width - Hitbox[LeftBody].Width), Position.Y);
                                }
                                else if (depth.X < 0) // right wall collision
                                {
                                    if (Hitbox[RightBody].Intersects(tileBound))
                                        Position = new Vector2(tileBound.X - (bounds.Width - Hitbox[RightBody].Width) + 2, Position.Y);
                                }
                                // update the bound of the player to re-check the neighboring tile
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }
            
            // checks if player on collision tile or not yet jump to avoid checking when player jumps
            if (CurrentAction == PlayerStatus.Falling || CurrentAction != PlayerStatus.Jump)
                CollisionForFallingTile(bounds);

            cayoteTime -= DeltaTime;
            previousPos = Position.Y;
            previousLeft = leftTile;
            previousBottom = bottomTile;
        }

        private void CollisionForFallingTile(Rectangle bounds)
        {
            foreach (var tile in level.FallingTiles)
            {
                HandleCollision(bounds, tile.BoundingRec);
            }
        }

        private void HandleCollision(Rectangle bounds, Rectangle tileBounds)
        {
            if (Hitbox[LeftBody].Intersects(tileBounds))
            {
                Position = new Vector2(tileBounds.X + (bounds.Width - Hitbox[LeftBody].Width), bounds.Y);
                collisionDirection = -1;
                playerSpeed.X = 0;
            }

            if (Hitbox[RightBody].Intersects(tileBounds))
            {
                Position = new Vector2(tileBounds.X - (bounds.Width - Hitbox[RightBody].Width) + 2, bounds.Y);
                collisionDirection = 1;
                playerSpeed.X = 0;
            }

            if (Hitbox[Head].Intersects(tileBounds))
            {
                headIsColliding = true;
                playerSpeed.Y = 0f;
                Position = new Vector2(Position.X, tileBounds.Y + tileBounds.Height * 0.8f);
            }

            if (Hitbox[Feet].Intersects(tileBounds))
            {
                cayoteTime = 0.1f;
                isGround = true;
                isJumping = false;
                Position = new Vector2(Position.X, tileBounds.Y - bounds.Height + 1);
                playerSpeed.Y = 0f;
            }
        }
        static TKey GetKeyByValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TValue value)
        {
            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                if (EqualityComparer<TValue>.Default.Equals(pair.Value, value))
                {
                    return pair.Key;
                }
            }
            return default; // or throw an exception if the value is not found
        }
        private void EnforceGravity(GameTime gameTime)
        {
            float delta = (float)(gameTime.ElapsedGameTime.TotalSeconds / (1.0 / 60));
            headIsColliding = false;
            if (!isGround)
            playerSpeed.Y += forces.Y;

            Position = new Vector2(Position.X + playerSpeed.X * delta, Position.Y + playerSpeed.Y * delta);

            Rectangle newBoundRectangle = new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                BoundingRectangle.Width,
                BoundingRectangle.Height
            );

            // Check for collisions
            GetCollision(newBoundRectangle, Position);

            // Reset isJumping if player is not in the air
            if (isGround)
            {
                isJumping = false;
            }

            if (Position.Y > 800)
            {
                isAlive = false;
            }
        }

        public void SetPlayerSpawn(Vector2 position)
        {
            spawnArea = position;
            Position = spawnArea;
        }
        public enum PlayerStatus
        {
            Idle = 0,
            Walking = 1,
            Jump = 2,
            Falling = 3,
            Attacking = 4
        }
    }
}

