using Fallen_Knight.GameAssets.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Fallen_Knight.GameAssets.Layers
{
    public class Layer
    {
        /// <summary>
        /// Background Texture
        /// </summary>
        Texture2D bg0Tx;
        Texture2D bg1Tx;
        Texture2D bg2Tx;
        Texture2D bg3Tx;
        /// <summary>
        /// Background layer rectangles
        /// </summary>
        Rectangle bg0;
        Rectangle bg1;
        Rectangle bg2;
        Rectangle bg3;
        /// <summary>
        /// Background layer positions
        /// </summary>
        Vector2 Bg0Pos;
        Vector2 Bg1Pos;
        Vector2 Bg2Pos;
        Vector2 Bg3Pos;

        Vector2 screenSize;
        float scrollSpeed;
        Level level;

        public void Load(ContentManager contentManager, Vector2 screenSize, Level level)
        {
            scrollSpeed = 200f;
            this.screenSize = screenSize;
            this.level = level;

            bg0Tx = contentManager.Load<Texture2D>("Scene/back");
            bg1Tx = contentManager.Load<Texture2D>("Scene/parallax-demon-woods-close-trees");
            bg2Tx = contentManager.Load<Texture2D>("Scene/parallax-demon-woods-far-trees");
            bg3Tx = contentManager.Load<Texture2D>("Scene/parallax-demon-woods-mid-trees");

            Bg0Pos = new Vector2(0, 0);
            Bg1Pos = new Vector2(0, 0);
            Bg2Pos = new Vector2(0, 0);
            Bg3Pos = new Vector2(0, 0);

            bg0 = new Rectangle((int)Bg0Pos.X, (int)Bg0Pos.Y, 1280, 720);
            bg1 = new Rectangle((int)Bg0Pos.X, (int)Bg0Pos.Y, 1280, 720);
            bg2 = new Rectangle((int)Bg0Pos.X, (int)Bg0Pos.Y, 1280, 720);
            bg3 = new Rectangle((int)Bg0Pos.X, (int)Bg0Pos.Y, 1280, 720);
        }

        public void BackgroundDraw(GameTime gameTime, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(bg0Tx, new Vector2(0, 0), new Rectangle(0, 0, 1280, 720), Color.AliceBlue);
            spriteBatch.End();
        }

    }
}
