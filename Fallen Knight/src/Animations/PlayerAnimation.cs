using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fallen_Knight.GameAssets.Animations
{
    public class PlayerAnimation : IAnimate
    {
        public GraphicsDevice graphics;
        Texture2D fadeScreen;
        float screenFadeDuration = 2f;

        public void Initialize(GraphicsDevice graphics)
        {
            fadeScreen = new Texture2D(graphics, 1, 1);
            fadeScreen.SetData(new Color[] { Color.Black });
        }
        public void DrawScreenFade()
        {
        }
        public void Update(GameTime gameTime)
        {
           screenFadeDuration -=  (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Texture);
            spriteBatch.Draw(fadeScreen, new Rectangle(0,0,1280, 720), Color.Black * screenFadeDuration);
            spriteBatch.End();
        }

    }

    public interface IAnimate
    {
        public void Initialize(GraphicsDevice graphics);
        public void Update(GameTime gameTime);
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
