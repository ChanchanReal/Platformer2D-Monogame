using Fallen_Knight.src.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Fallen_Knight.GameAssets.Animations
{
    public class PlayerAnimation
    {
        GraphicsDevice graphics;
        Texture2D fadeScreen;
        float screenFadeDuration = 2f;
        FaceDirection spriteDirection = FaceDirection.Right;
        IAnimate currentAnimation;
        List<IAnimate> animations;

        public Point IdleSize = new Point(idleFrameWidth, idleFrameHeight);
        private static int idleFrameWidth = 64;
        private static int idleFrameHeight = 64;
        public Rectangle PlayerPosition
        {
            get { return playerPosition; }
        }
        private Rectangle playerPosition;
        public float DeltaTime
        {
            get { return deltaTime; }
        }
        private float deltaTime;
        public PlayerStatus PlayerStatus
        {
            get { return playerStatus; }
        }
        private PlayerStatus playerStatus;
        public void Initialize(GraphicsDevice graphics, ContentManager contentManager)
        {
            fadeScreen = new Texture2D(graphics, 1, 1);
            fadeScreen.SetData(new Color[] { Color.Black });
            
        }
        public void CreateAnimation(ContentManager contentManager)
        {
            animations = new List<IAnimate>
            {
                new IdleAnimation(contentManager.Load<Texture2D>("Player/idlemain"), 64, 64),
                new RunAnimation(contentManager.Load<Texture2D>("Player/runmain"), 63, 64),
                new AttackAnimation(contentManager.Load<Texture2D>("Player/idlemain"), 64, 64),
                new JumpAnimation(contentManager.Load<Texture2D>("Player/jumpmain"), 64, 64),
                new FallAnimation(contentManager.Load<Texture2D>("Player/fallmain"), 64, 64),
            };
            
        }
        public void Update(GameTime gameTime, Rectangle position, FaceDirection faceDirection, PlayerStatus playerState)
        {
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            screenFadeDuration -= DeltaTime;
            playerPosition = position;
            this.spriteDirection = faceDirection;
            this.playerStatus = playerState;

            currentAnimation = GetCurrentAnimation(playerState);

            if (currentAnimation != null)
            {
                Animation animation = (Animation)currentAnimation;
                animation.FlipH = FaceDirectionToBool(faceDirection);
                animation.Position = PlayerPosition;
                currentAnimation.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (currentAnimation != null)
                currentAnimation.Draw(spriteBatch, gameTime);
        }

        private bool FaceDirectionToBool(FaceDirection faceDirection)
        {
            return faceDirection == FaceDirection.Left? true : false;
        }

        public IAnimate GetCurrentAnimation(PlayerStatus playerState)
        {
            return playerState switch
            {
                PlayerStatus.Idle => animations[0],
                PlayerStatus.Walk => animations[1],
                PlayerStatus.Attack => animations[2],
                PlayerStatus.Jump => animations[3],
                PlayerStatus.Fall => animations[4],
                _ => animations[0]
            };
        }
    }

    public interface IAnimate
    {
        public void Update(GameTime gameTime);
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }

    public class IdleAnimation : Animation, IAnimate
    {
        public IdleAnimation(Texture2D texture, int sizeX, int sizeY) : base(texture, sizeX, sizeY)
        {
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch);
        }

        public void Initialize(GraphicsDevice graphics)
        {
            
        }

        public void Update(GameTime gameTime)
        {
            base.UpdateFrame(gameTime);
        }
    }
    public class RunAnimation : Animation, IAnimate
    {
        public RunAnimation(Texture2D texture, int sizeX, int sizeY) : base(texture, sizeX, sizeY)
        {
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch);
        }

        public void Initialize(GraphicsDevice graphics)
        {

        }

        public void Update(GameTime gameTime)
        {
            base.UpdateFrame(gameTime);
        }
    }
    public class AttackAnimation : Animation, IAnimate
    {
        public AttackAnimation(Texture2D texture, int sizeX, int sizeY) : base(texture, sizeX, sizeY)
        {
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch);
        }

        public void Initialize(GraphicsDevice graphics)
        {

        }

        public void Update(GameTime gameTime)
        {
            base.UpdateFrame(gameTime);
        }
    }
    public class JumpAnimation : Animation, IAnimate
    {
        public JumpAnimation(Texture2D texture, int sizeX, int sizeY) : base(texture, sizeX, sizeY)
        {
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch);
        }

        public void Initialize(GraphicsDevice graphics)
        {

        }

        public void Update(GameTime gameTime)
        {
            //we only need one frame so im disabling this for now
            //base.UpdateFrame(gameTime);
        }
    }
    public class FallAnimation : Animation, IAnimate
    {
        public FallAnimation(Texture2D texture, int sizeX, int sizeY) : base(texture, sizeX, sizeY)
        {
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch);
        }

        public void Initialize(GraphicsDevice graphics)
        {

        }

        public void Update(GameTime gameTime)
        {
            //we only need one frame so im disabling this for now
            //base.UpdateFrame(gameTime);
        }
    }
}
