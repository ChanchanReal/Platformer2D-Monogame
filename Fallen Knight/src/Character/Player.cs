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
        #region GameStates & Animation
        private PlayerAnimation playerAnimation;
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
        private float dashCoolDown = 0f;
        private float dashTime = 0.2f;
        private float dashDuration = 0f;
        private const float dashSpeed = 15;
        private bool isDashing = false;
        private bool spriteDirection = false;
        private bool isJumping = false;
        private bool wantsToJump = false;
        private bool headIsColliding = false;
        private int collisionDirection = 0;

        private ICommand oldCommand;
        // Hit box for side i don't know
        private const int Head = 0;
        private const int LeftBody = 1;
        private const int RightBody = 2;
        private const int Feet = 3;
        public Rectangle[] Hitbox;
        #endregion

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
                int left = (int)Math.Round(Position.X + (int)0.2f * playerAnimation.IdleSize.X);
                int top = (int)Math.Round(Position.Y + (int)0.2f * playerAnimation.IdleSize.Y);

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

        public Player(Level level, ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            this.level = level;
            this.contentManager = contentManager;
            isAlive = true;
            LoadContent(graphicsDevice);
        }

        private const int PlayerDefaultWidth = 64;
        private const int PlayerAttackingWidth = 125;
        private const int SpriteHeight = 86;

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            playerAnimation = new PlayerAnimation();
            playerAnimation.CreateAnimation(contentManager);
            Position = SpawnPoint;
            isAlive = true;

            int width = (int)(playerAnimation.IdleSize.X * 0.4);
            int left = (playerAnimation.IdleSize.X - width) / 2;
            int height = (int)(playerAnimation.IdleSize.Y * 0.8);
            int top = playerAnimation.IdleSize.Y- height;

            boundRectangle = new Rectangle((int)Position.X - PlayerAttackingWidth, (int)Position.Y, PlayerDefaultWidth, SpriteHeight);
            List<Texture2D> texture = new List<Texture2D>();
            texture.Add(contentManager.Load<Texture2D>("diamond"));
            Texture2D text = new Texture2D(graphicsDevice, 1, 1);
            text.SetData(new Color[] { Color.White });
            texture.Add(text);
            particleSystem = new ParticleSystem(Position, texture);
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
        #region Character Update and Inputs
        public void Update(GameTime gameTime)
        {
            SetDeltaTime(gameTime);
            movement = 0;

            if (CurrentAction == PlayerStatus.Attack)
            {
                attackTimer -= DeltaTime;
                if (attackTimer <= 0)
                {
                    CurrentAction = PlayerStatus.Idle;
                    attackTimer = 0;
                }
            }

            playerAnimation.Update(gameTime, BoundingRectangle, spriteDirection, CurrentAction);

            GetInput(gameTime);
            EnforceGravity(gameTime);
            SwitchCurrentActionJump();
            CharacterDash();
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
                CurrentAction = PlayerStatus.Fall;
            }
        }
        private void CharacterDash()
        {

            dashCoolDown -= DeltaTime;

            if (!isDashing)
                return;

            if (dashDuration > 0)
            {
                dashDuration -= DeltaTime;
                if (spriteDirection == false)
                {
                    playerSpeed = new Vector2(dashSpeed, 0);
                }
                else
                {
                    playerSpeed = new Vector2(-dashSpeed, 0);
                }
            }
            else
            {
                dashDuration = 0;
                isDashing = false;
                if (spriteDirection)
                {
                    playerSpeed = new Vector2(-maxWalkingSpeed, playerSpeed.Y);
                }
                else
                {
                    playerSpeed = new Vector2(maxWalkingSpeed, playerSpeed.Y);
                }
                // Reset speed after dash
            }
        }
        private void GetInput(GameTime gameTime)
        {
            ICommand input = HandleInput(gameTime);

            if (input != null)
            {
                playerSpeed = input.Execute(gameTime);
                input.GetPlayerState(CurrentAction);
                CurrentAction = input.UpdatePlayerState();
            }

            if (InputManager.Input(Keys.F))
            {
                CurrentAction = PlayerStatus.Attack;
                attackTimer = AttackDelay;
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
            else if ( dashCoolDown <= 0 && InputManager.Input(Keys.D)
                && (collisionDirection != -1 ||
                collisionDirection != 1))
            {
                isDashing = true;
                dashDuration = dashTime;
                GenerateDashParticle(gameTime);
                dashCoolDown = 2f;
            }
            if (InputManager.Input(Keys.K))
            {
                Position = SpawnPoint;
            }

            if (wantsToJump && wantsToJumpDuration <= 0)
            {
                wantsToJump = false;
            }

            if (isGround && !headIsColliding && (wantsToJump || InputManager.Input(Keys.Space)))
            {
                isGround = false;
                isJumping = true;
                wantsToJump = false;
                wantsToJumpDuration = 0f;
                CurrentAction = PlayerStatus.Jump;
                return new Jump(maxWalkingSpeed, accel, jumpSpeed, playerSpeed, playerAnimation);
            }

            // Decrement wantsToJumpDuration regardless of whether a jump occurred
            wantsToJumpDuration -= DeltaTime;

            if (InputManager.HoldableInput(Keys.Right) && collisionDirection != 1)
            {
                spriteDirection = false;
                CurrentAction = PlayerStatus.Walk;
                GenerateParticles(gameTime);
                if (!isDashing)
                    return new MoveRight(maxWalkingSpeed, accel, jumpSpeed, playerSpeed, playerAnimation);
            }
            else if (InputManager.HoldableInput(Keys.Left) && collisionDirection != -1)
            {
                spriteDirection = true;
                CurrentAction = PlayerStatus.Walk;
                GenerateParticles(gameTime);
                if (!isDashing)
                    return new MoveLeft(maxWalkingSpeed, accel, jumpSpeed, playerSpeed, playerAnimation);
            }
            else
            {
                ApplyFriction();

                if (playerSpeed.X == 0 && collisionDirection == 0)
                {
                    CurrentAction = PlayerStatus.Idle;
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
        private void GenerateDashParticle(GameTime gameTime)
        {
            particleSystem.GenerateDashParticles();
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
        #endregion
        public void Draw(SpriteBatch sprite, GameTime gameTime)
        {
            playerAnimation.Draw(sprite, gameTime);
            particleSystem.Draw(sprite);
        }
        #region Collision Handling
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
                    TileType collision = level.GetCollision(x, y);
                    if (collision == TileType.Platform)
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
                                headIsColliding = true;
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
                                if (depth.X > 0 || collision == TileType.Impassable)
                                {
                                    if (Hitbox[LeftBody].Intersects(tileBound))
                                    {
                                        playerSpeed.X = 0;
                                        collisionDirection = -1;
                                        Position = new Vector2(tileBound.X + (bounds.Width - Hitbox[LeftBody].Width), Position.Y);
                                    }
                                }
                                else if (depth.X < 0 || collision == TileType.Impassable) // right wall collision
                                {
                                    if (Hitbox[RightBody].Intersects(tileBound))
                                    {
                                        playerSpeed.X = 0;
                                        collisionDirection = 1;
                                        Position = new Vector2(tileBound.X - (bounds.Width - Hitbox[RightBody].Width) + 2, Position.Y);
                                    }
                                }
                                // update the bound of the player to re-check the neighboring tile
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }
            
            // checks if player on collision tile or not yet jump to avoid checking when player jumps
            if (CurrentAction == PlayerStatus.Fall || CurrentAction != PlayerStatus.Jump)
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
        #endregion
        public void SetPlayerSpawn(Vector2 position)
        {
            spawnArea = position;
            Position = spawnArea;
        }
        public enum PlayerStatus
        {
            Idle = 0,
            Walk = 1,
            Jump = 2,
            Fall = 3,
            Attack = 4,
            Dash = 5,
        }
    }
}

