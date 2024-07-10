using Fallen_Knight.GameAssets.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using static Fallen_Knight.GameAssets.Character.Player;

namespace Fallen_Knight
{
    public class DebugHelper
    {
        private Texture2D squareTexture;
        private Texture2D circleTexture;
        private SpriteFont spriteFont;
        private List<Rectangle> debugRectangles;
        private List<Circle> itemBound;
        private float gameTime = 0f;

        bool isDebug = false;
        bool showHitbox = false;
        bool showGameTime = false;
        PlayerStatus status = PlayerStatus.Idle;

        int spacing = 0;

        KeyboardState oldKeystate;

        public void Load(ContentManager content)
        {
            spriteFont = content.Load<SpriteFont>("Font/text");
            squareTexture = content.Load<Texture2D>("hitbox_square64");
            circleTexture = content.Load<Texture2D>("hitbox_circle64");
        }

        public void Update(GameTime gameTime, KeyboardState keyState, List<Rectangle> target, List<Circle> circles)
        {
            float delta = (float)gameTime.TotalGameTime.TotalSeconds;
            debugRectangles = target;
            itemBound = circles;

            if (keyState.IsKeyDown(Keys.F1) && !oldKeystate.IsKeyDown(Keys.F1))
            {
                isDebug = !isDebug;
                Console.WriteLine(isDebug);
            }

            if (isDebug && keyState.IsKeyDown(Keys.F2) && !oldKeystate.IsKeyDown(Keys.F2))
            {
                showHitbox = !showHitbox;
            }

            if (isDebug && keyState.IsKeyDown(Keys.F3) && !oldKeystate.IsKeyDown(Keys.F3))
            {
                showGameTime = !showGameTime;
            }

            this.gameTime = delta;
            oldKeystate = keyState;
        }

        public void DrawDebugRectangle(SpriteBatch spriteBatch)
        {
            int i = 0;
            if (showHitbox)
            {
                foreach (var rect in debugRectangles)
                {
                    spriteBatch.Draw(squareTexture, rect, Color.LightGreen);
                    i++;
                }

                DrawItemBound(spriteBatch);
            }
        }

        public void DrawItemBound(SpriteBatch sb)
        {
            foreach (var circle in itemBound)
            {
                float scaleFactor = (circle.Radius * 2) / 64f;
                Vector2 origin = new Vector2(64 / 2, 64 / 2);
                sb.Draw(circleTexture, new Vector2(circle.Center.X, circle.Center.Y),
                    new Rectangle(0,0, 64, 64), Color.DarkRed, 0f, origin, scaleFactor, SpriteEffects.None, 0f);
            }
        }

        public void GetCurrentAction(PlayerStatus playerStatus)
        {
            this.status = playerStatus;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spacing = 0;
            spriteBatch.Begin();
            if (isDebug)
            {
                DrawText(spriteBatch, $"Game Time {gameTime}", 0);
                DrawText(spriteBatch, $"Debug is on {isDebug}", 1);
                DrawText(spriteBatch, $"Show Hitbox F1 - {showHitbox}", 2);
                DrawText(spriteBatch, $"Player action - {status}", 3);
            }

            spriteBatch.End();
        }

        private void DrawText(SpriteBatch spriteBatch, string txt, int id)
        {
            spriteBatch.DrawString(spriteFont, txt, new Vector2(0, spacing), Color.White);
            spacing += 20;
        }
    }
}
