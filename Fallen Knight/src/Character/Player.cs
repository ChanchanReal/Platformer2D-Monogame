using Fallen_Knight.GameAssets.Animations;
using Fallen_Knight.GameAssets.Collisions;
using Fallen_Knight.GameAssets.Interface;
using Fallen_Knight.GameAssets.Levels;
using Fallen_Knight.GameAssets.Mobs;
using Fallen_Knight.GameAssets.Tiles;
using Fallen_Knight.src.Core;
using Fallen_Knight.src.Interface;
using Fallen_Knight.src.Items;
using Fallen_Knight.src.PlayerState;
using Fallen_Knight.src.Score;
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
        #region GameStates & Animation
        private ParticleSystem particleSystem;
        private ContentManager contentManager;
        public PlayerAnimation PlayerAnimation;
        public Animation BladeParticle;
        public Weapon playerWeapon;

        // Game physics
        public Vector2 PlayerSpeed = new Vector2(0f, 0f);
        private static float accel = 0.1f;
        private static float friction = accel * 3f;
        private static float tolerance = friction * 0.9f;
        private const float RunSpeed = 300f;
        private const float dashSpeed = 15;
        // Hit box for side i don't know
        private const int LeftBody = 1;
        private const int RightBody = 2;
        private const int Feet = 3;
        private const int Head = 0;
        public const float MaxWalkingSpeed  = 6f;       
        // gravity
        private static float gravity = 9.8f / 40;
        private Vector2 forces = new Vector2(friction, gravity);
        public IPlayerState PlayerState;

        // Player relation checks
        public float AttackTime = 0.3f;
        public Attack AttackStatus = Attack.Off;
        public int CollisionDirection { get; set; } = 0;
        public PlayerStatus CurrentAction;
        public float WantsToJumpDuration { get; set; } = 0f;
        public float CayoteTime { get; set; } = 0f;
        public float Movement { get; set; } = 0;
        public float dashCoolDown { get; set; } = 0f;
        public float DashTime { get; set; } = 0.2f;
        public float DashDuration { get; set; } = 0f;
        public bool IsDashing { get; set; } = false;
        public bool SpriteDirection { get; set; } = false;
        public bool IsJumping { get; set; } = false;
        public bool WantsToJump { get; set; } = false;
        public bool HeadIsColliding { get; set; } = false;
        public bool IsGround { get; set; } = false;
        #endregion
        public Rectangle[] Hitbox;

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
                int left = (int)Math.Round(Position.X + (int)0.2f * PlayerAnimation.IdleSize.X);
                int top = (int)Math.Round(Position.Y + (int)0.2f * PlayerAnimation.IdleSize.Y);

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

        public Player(Level level,
            ContentManager contentManager, 
            GraphicsDevice graphicsDevice,
            ParticleSystem particleSystem)
        {
            this.level = level;
            this.contentManager = contentManager;
            this.particleSystem = particleSystem;
            isAlive = true;
            LoadContent(graphicsDevice);
        }

        private const int PlayerDefaultWidth = 64;
        private const int SpriteHeight = 64;

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            PlayerAnimation = new PlayerAnimation();
            PlayerAnimation.CreateAnimation(contentManager);
            BladeParticle = new Animation(Level.Content.Load<Texture2D>("WeaponEffect/katana attack"), 64, 32);
            PlayerState = new Idle(this);
            Position = SpawnPoint;
            isAlive = true;

            int width = (int)(PlayerAnimation.IdleSize.X * 0.4);
            int left = (PlayerAnimation.IdleSize.X - width) / 2;
            int height = (int)(PlayerAnimation.IdleSize.Y * 0.8);
            int top = PlayerAnimation.IdleSize.Y- height;

            boundRectangle = new Rectangle((int)Position.X - PlayerDefaultWidth, (int)Position.Y, PlayerDefaultWidth, SpriteHeight);
            playerWeapon = new Katana(contentManager.Load<Texture2D>("Item/katana"),
                contentManager.Load<Texture2D>("Item/katana"),
                new Rectangle(400, 660, 60, 30));
            LoadHitBox();
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
            int topHitboxY = BoundingRectangle.Y + (int)(BoundingRectangle.Height * 0.1f);

            Hitbox[Head] = new Rectangle(topHitboxX, topHitboxY - 5, topHitboxWidth, topHitboxHeight);

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
            int feetY = rightBodyY + ((rightBodyHeight + feetHeight) / 2) + 7;

            int updateFeetY = feetY - (int)(feetY * 0.01f);

            Hitbox[Feet] = new Rectangle(feetX, updateFeetY, feetWidth, feetHeight);
        }
        #region Character Update and Inputs
        public void Update(GameTime gameTime)
        {
            SetDeltaTime(gameTime);
            Movement = 0;

            DebugHelper.AddToDebugBound(BoundingRectangle, 0);
            DebugHelper.AddToDebugBound(Hitbox[Feet], 1);
            DebugHelper.AddToDebugBound(Hitbox[Head], 2);
            DebugHelper.AddToDebugBound(Hitbox[LeftBody], 3);
            DebugHelper.AddToDebugBound(Hitbox[RightBody], 4);
            DebugHelper.GetCurrentAction(CurrentAction);
            DebugHelper.GetVelocity(PlayerSpeed);

            PlayerAnimation.Update(gameTime, BoundingRectangle, SpriteDirection, CurrentAction);
            PlayerState.HandleInput(gameTime);
            PlayerState.UpdateState();

            GetInput(gameTime);
            EnforceGravity(gameTime);
            IsCharacterDead();
            LoadHitBox();
            particleSystem.Update(this);
            playerWeapon.Update(gameTime, BoundingRectangle);
            ResetWantsToJump();
            AttackEnabled(gameTime);

            playerWeapon.flipH = SpriteDirection;
            
            if (AttackStatus == Attack.On)
            {
                BladeParticle.UpdateFrame(gameTime);
                BladeParticle.Position = playerWeapon.AttackHitBox;
                BladeParticle.FlipH = SpriteDirection;
            }

            // Decrement wantsToJumpDuration regardless of whether a jump occurred
            WantsToJumpDuration -= DeltaTime;
            dashCoolDown -= DeltaTime;
        }
        private void ResetWantsToJump()
        {
            if (WantsToJump && WantsToJumpDuration <= 0)
            {
                WantsToJump = false;
            }
        }

        public void IsCharacterDead()
        {
            if (!IsAlive)
            {
                Position = spawnArea;
                PlayerSpeed = Vector2.Zero;
                isAlive = true;
            }
        }
        
        private void GetInput(GameTime gameTime)
        {

            if (InputManager.Input(Keys.F))
            {
                AttackTime = 0.3f;
                AttackStatus = Attack.On;
                Console.WriteLine("Trying To Attack");
            }
        }

        private void AttackEnabled(GameTime gameTime)
        {
            if (AttackTime > 0f)
            {
                HitEnemyCheck();
                AttackTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (AttackTime < 0)
            {
                AttackTime = 0f;
                AttackStatus = Attack.Off;
            }
        }

        private void HitEnemyCheck()
        {
            foreach (var enemy in Level.enemies)
            {
                Enemy mob = enemy as Enemy;
                if (mob.BoundingRectangle.Intersects(playerWeapon.AttackHitBox))
                {
                    mob.KillEnemy();
                }
            }
        }
        private void SetDeltaTime(GameTime gameTime)
        {
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        public void SwitchState(IPlayerState playerState)
        {
            this.PlayerState = playerState;
        }
        public void ApplyFriction()
        {
            if (IsGround)
            {
                PlayerSpeed.X += -Math.Sign(PlayerSpeed.X) * forces.X;

                if (Math.Abs(PlayerSpeed.X) <= tolerance)
                {
                    PlayerSpeed.X = 0;
                }
            }
        }
        public void GenerateDashParticle(GameTime gameTime)
        {
            particleSystem.GenerateDashParticles(PlayerSpeed);
        }
        public void GenerateParticles(GameTime gameTime)
        {
            float elapse = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!IsGround)
                return;

            if (particleSystem.DelayTime >= 0)
            {
                particleSystem.DelayTime -= elapse;
            }
            else
            {
                particleSystem.DelayTime = 0.1f;
                if (PlayerSpeed.X != 0)
                particleSystem.StartGeneratingParticle();
            }
        }
        #endregion
        public void Draw(SpriteBatch sprite, GameTime gameTime)
        {
            PlayerAnimation.Draw(sprite, gameTime);
            playerWeapon.Draw(gameTime, sprite);
            
            if (AttackStatus == Attack.On)
            BladeParticle.Draw(sprite);
        }

        public void DrawPlayerEffect(SpriteBatch sprite, GameTime gameTime , Matrix camera)
        {
            particleSystem.Draw(sprite, camera);
        }
        #region Collision Handling
        private float previousBottom;
        private float previousLeft;
        private float previousPos;
        private void GetCollision(Rectangle newBoundRectangle, Vector2 velocity)
        {
            Rectangle bounds = newBoundRectangle;

            if (CayoteTime <= 0)
                IsGround = false;

            CollisionDirection = 0;

            // gets the player bounds in game world
            int leftTile = (int)Math.Floor((float)Hitbox[LeftBody].Left / 64);
            int rightTile = (int)Math.Ceiling(((float)Hitbox[RightBody].Right / 64)) - 1;
            int topTile = (int)Math.Floor((float)Hitbox[Head].Top / 64); // Correct tile size to 64
            int bottomTile = (int)Math.Ceiling(((float)Hitbox[Feet].Bottom / 64)) - 1; // Correct tile size to 64

            // checks for the closest tile between the player
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    TileType collision = level.GetCollision(x, y);
                    if (collision == TileType.Platform)
                    {
                        // gets the tile bound and depth of how player and tile intersects
                        Rectangle tileBound = Level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBound);
                        DebugHelper.AddToDebugBound(tileBound, 66);
                        
                        if (depth == Vector2.Zero)
                            return;

                        // check Height is less than width depth
                        if (Math.Abs(depth.Y) < Math.Abs(depth.X))
                        {
                            // if we are on top of tile.
                            if (previousBottom <= tileBound.Top &&  PlayerSpeed.Y > 0 && tileBound.Intersects(Hitbox[Feet]))
                            {
                                CayoteTime = 0.1f;
                                IsGround = true;
                                PlayerSpeed.Y = 0;
                                Position = new Vector2(Position.X, (Position.Y + depth.Y) + 3); // Add slight offset to ensure on top
                            }
                            else if (PlayerSpeed.Y < 0 && Hitbox[Head].Intersects(tileBound)) // if we hit the top of the tile as well as our velocity is jumping
                            {
                                // Hitting head on the ceiling
                                Position = new Vector2(Position.X, Position.Y + depth.Y);
                                PlayerSpeed.Y = 0; // Stop upward movement
                                HeadIsColliding = true;
                                Console.WriteLine("Head Hit Wall");
                            }
                            // update the bound of the player to re-check the neighboring tile
                            bounds = BoundingRectangle;
                        }
                        // Resolve horizontal collision
                        else
                        {
                            // making sure we collided with a wall collision tile.
                            if (depth.X != 0)
                            {
                                // if left wall as well as double checking to make sure we did intersect with our custom hitbox
                                if (depth.X > 0 || collision == TileType.Impassable)
                                {
                                    if (Hitbox[LeftBody].Intersects(tileBound))
                                    {
                                        PlayerSpeed.X = 0;
                                        CollisionDirection = -1;
                                        Position = new Vector2(tileBound.X + (bounds.Width - Hitbox[LeftBody].Width), Position.Y);
                                        if (IsDashing)
                                        {
                                            SwitchState(new Idle(this));
                                            IsDashing = false;
                                        }
                                    }
                                }
                                else if (depth.X < 0 || collision == TileType.Impassable) // right wall collision
                                {
                                    if (Hitbox[RightBody].Intersects(tileBound))
                                    {
                                        PlayerSpeed.X = 0;
                                        CollisionDirection = 1;
                                        Position = new Vector2(tileBound.X - (bounds.Width - Hitbox[RightBody].Width) + 2, Position.Y);
                                        if (IsDashing)
                                        {
                                            SwitchState(new Idle(this));
                                            IsDashing = false;
                                        }
                                    }
                                }
                                // update the bound of the player to re-check the neighboring tile
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }

            if (PlayerState is not Jump)
                CollisionForFallingTile(bounds);

            CayoteTime -= DeltaTime;
            previousPos = Position.Y;
            previousLeft = leftTile;
            previousBottom = bottomTile;
        }

        private void CollisionForFallingTile(Rectangle bounds)
        {
            if (level.FallingTiles != null)
            foreach (var tile in level.FallingTiles)
            {
                if (tile is FallingTile)
                {
                    FallingTile fallingTile = (FallingTile)tile;

                    HandleCollision(bounds, fallingTile.BoundingRectangle);
                }
                
            }
        }
        
        private void HandleCollision(Rectangle bounds, Rectangle tileBounds)
        {
            if (Hitbox[LeftBody].Intersects(tileBounds))
            {
                Position = new Vector2(tileBounds.X + (bounds.Width - Hitbox[LeftBody].Width), bounds.Y);
                CollisionDirection = -1;
                PlayerSpeed.X = 0;
            }

            if (Hitbox[RightBody].Intersects(tileBounds))
            {
                Position = new Vector2(tileBounds.X - (bounds.Width - Hitbox[RightBody].Width), bounds.Y);
                CollisionDirection = 1;
                PlayerSpeed.X = 0;
            }

            if (Hitbox[Head].Intersects(tileBounds))
            {
                HeadIsColliding = true;
                PlayerSpeed.Y = 0f;
                Position = new Vector2(Position.X, tileBounds.Y + tileBounds.Height * 2);
            }

            if (Hitbox[Feet].Intersects(tileBounds))
            {
                CayoteTime = 0.1f;
                IsGround = true;
                IsJumping = false;
                Position = new Vector2(Position.X, tileBounds.Y - bounds.Height + 3);
                PlayerSpeed.Y = 0f;
            }

            LoadHitBox();
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
            HeadIsColliding = false;

            if (!IsGround)
            PlayerSpeed.Y += forces.Y;

            Position = new Vector2(Position.X + PlayerSpeed.X * delta, Position.Y + PlayerSpeed.Y * delta);

            Rectangle newBoundRectangle = new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                BoundingRectangle.Width,
                BoundingRectangle.Height
            );

            // Check for collisions
            GetCollision(newBoundRectangle, Position);

            // Reset isJumping if player is not in the air
            if (IsGround)
            {
                IsJumping = false;
            }

            if (Position.Y > 1020)
            {
                SetPlayerToDead();
            }
        }
        #endregion

        public void SetPlayerToDead()
        {
            isAlive = false;
            Score.SubtractScore(15);
        }
        public void SetPlayerSpawn(Vector2 position)
        {
            spawnArea = position;
            Position = spawnArea;
        }

        public enum Attack { On, Off}
    }
}

