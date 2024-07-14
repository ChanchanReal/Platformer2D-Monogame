using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fallen_Knight.GameAssets.Animations
{
    public class Animation
    {
        private Texture2D texture;
        private int maxFrame;
        private int width;
        private int height;
        private int currentFrame = 0;
        private float frameTime = 0.1f;
        private float timer = 0;
        private bool isLooping = false;

        public Rectangle Position;
        public bool FlipH = false;

        public Animation(Texture2D texture, int sizeX, int sizeY)
        {
            this.texture = texture;
            width = sizeX;
            height = sizeY;
            maxFrame = texture.Width / sizeX;
        }

        public int FrameWidth
        {
            get => height;
        }

        public int FrameHeight
        {
            get => height;
        }

        public void UpdateFrame(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= frameTime && !isLooping)
            {
                currentFrame++;
                if (currentFrame >= maxFrame)
                {
                    currentFrame = 0;
                }
                timer = 0;
            }
        }

        public void Draw(SpriteBatch sprite)
        {
            if (!FlipH)
            {
                sprite.Draw(texture,
                    Position,
                    GetCurrentFrame(),
                    Color.AliceBlue
                    );
            }
            else
            {
                sprite.Draw(
                    texture,
                    Position,
                    GetCurrentFrame(),
                    Color.AliceBlue,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.FlipHorizontally,
                    0
                    );
            }
        }

        private Rectangle GetCurrentFrame()
        {
            return new Rectangle(width * currentFrame, 0, width, height);
        }
    }
}
