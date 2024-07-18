using Fallen_Knight.GameAssets.Levels;
using Fallen_Knight.src.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Formats.Asn1.AsnWriter;

namespace Fallen_Knight.GameAssets.Layers
{
    public class Layer
    {
        Texture2D bg0Tx, bg1Tx, bg2Tx, bg3Tx;
        Rectangle bg0, bg1, bg2, bg3;
        Vector2 Bg0Pos, Bg1Pos, Bg2Pos, Bg3Pos;
        Vector2 mirrorBg0Pos, mirrorBg1Pos, mirrorBg2Pos, mirrorBg3Pos, mirrorBg4Pos;
        Vector2 screenSize;
        float scrollSpeedBg0, scrollSpeedBg1, scrollSpeedBg2, scrollSpeedBg3;
        Level level;

        public void Load(ContentManager contentManager, Vector2 screenSize, Level level)
        {
            this.screenSize = screenSize;
            this.level = level;

            scrollSpeedBg0 = 50f;
            scrollSpeedBg1 = 200f;
            scrollSpeedBg2 = 150f;
            scrollSpeedBg3 = 100;

            bg0Tx = contentManager.Load<Texture2D>("Scene/parallax-demon-woods-bg");
            bg1Tx = contentManager.Load<Texture2D>("Scene/parallax-demon-woods-close-trees");
            bg2Tx = contentManager.Load<Texture2D>("Scene/parallax-demon-woods-far-trees");
            bg3Tx = contentManager.Load<Texture2D>("Scene/parallax-demon-woods-mid-trees");

            Bg0Pos = new Vector2(0, 0);
            Bg1Pos = new Vector2(0, 0);
            Bg2Pos = new Vector2(0, 0);
            Bg3Pos = new Vector2(0, 0);

            mirrorBg0Pos = new Vector2(screenSize.X, 0);
            mirrorBg1Pos = new Vector2(screenSize.X, 0);
            mirrorBg2Pos = new Vector2(screenSize.X, 0);
            mirrorBg3Pos = new Vector2(screenSize.X, 0);
            mirrorBg4Pos = new Vector2(screenSize.X, 0);

            bg0 = new Rectangle((int)Bg0Pos.X, (int)Bg0Pos.Y, bg0Tx.Width, bg0Tx.Height);
            bg1 = new Rectangle((int)Bg1Pos.X, (int)Bg1Pos.Y, bg1Tx.Width, bg1Tx.Height);
            bg2 = new Rectangle((int)Bg2Pos.X, (int)Bg2Pos.Y, bg2Tx.Width, bg2Tx.Height);
            bg3 = new Rectangle((int)Bg3Pos.X, (int)Bg3Pos.Y, bg3Tx.Width, bg3Tx.Height);
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 direction = Vector2.Zero;

            if (InputManager.HoldableInput(Keys.Left))
            {
                direction = new Vector2(1, 0);
            }
            else if (InputManager.HoldableInput(Keys.Right))
            {
                direction = new Vector2(-1, 0);
            }

            Bg0Pos += direction * scrollSpeedBg0 * delta;
            Bg1Pos += direction * scrollSpeedBg1 * delta;
            Bg2Pos += direction * scrollSpeedBg2 * delta;
            Bg3Pos += direction * scrollSpeedBg3 * delta;

            mirrorBg0Pos = new Vector2(Bg0Pos.X + bg0Tx.Width, Bg0Pos.Y);
            mirrorBg1Pos = new Vector2(Bg1Pos.X + bg1Tx.Width, Bg1Pos.Y);
            mirrorBg2Pos = new Vector2(Bg2Pos.X + bg2Tx.Width, Bg2Pos.Y);
            mirrorBg3Pos = new Vector2(Bg3Pos.X + bg3Tx.Width, Bg3Pos.Y);
            mirrorBg4Pos = new Vector2(Bg1Pos.X - bg1Tx.Width, Bg1Pos.Y);

            ResetPosition();
        }

        private void ResetPosition()
        {
            if (Bg0Pos.X <= -bg0Tx.Width)
            {
                Bg0Pos = new Vector2(bg0Tx.Width, 0);
            }
            if (Bg1Pos.X <= -bg1Tx.Width)
            {
                Bg1Pos = new Vector2(bg1Tx.Width, 0);
            }
            if (Bg2Pos.X <= -bg2Tx.Width)
            {
                Bg2Pos = new Vector2(bg2Tx.Width, 0);
            }
            if (Bg3Pos.X <= -bg3Tx.Width)
            {
                Bg3Pos = new Vector2(bg3Tx.Width, 0);
            }

            if (mirrorBg0Pos.X <= 0)
            {
                mirrorBg0Pos = new Vector2(Bg0Pos.X + bg0Tx.Width, 0);
            }
            if (mirrorBg1Pos.X <= 0)
            {
                mirrorBg1Pos = new Vector2(Bg1Pos.X + bg1Tx.Width, 0);
            }
            if (mirrorBg2Pos.X <= 0)
            {
                mirrorBg2Pos = new Vector2(Bg2Pos.X + bg2Tx.Width, 0);
            }
            if (mirrorBg3Pos.X <= 0)
            {
                mirrorBg3Pos = new Vector2(Bg3Pos.X + bg3Tx.Width, 0);
            }

        }

        public void BackgroundDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float scale = 3f;
            spriteBatch.Begin();
            spriteBatch.Draw(bg0Tx, Bg0Pos,null ,Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(bg0Tx, mirrorBg0Pos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(bg3Tx, Bg3Pos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(bg3Tx, mirrorBg3Pos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(bg2Tx, Bg2Pos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(bg2Tx, mirrorBg2Pos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(bg1Tx, mirrorBg1Pos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(bg1Tx, Bg1Pos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(bg1Tx, mirrorBg4Pos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.End();
        }
    }
}
