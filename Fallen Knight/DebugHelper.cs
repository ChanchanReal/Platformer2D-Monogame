using Fallen_Knight.GameAssets.Collisions;
using Fallen_Knight.GameAssets.Tile.Tile;
using Fallen_Knight.src.Core;
using Fallen_Knight.src.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using static Fallen_Knight.GameAssets.Character.Player;

namespace Fallen_Knight
{
#if DEBUG
    public static class DebugHelper
    {
        private static Texture2D squareTexture;
        private static Texture2D circleTexture;
        private static SpriteFont spriteFont;
        private static List<Circle> itemBound;
        private static List<FallingTile> fallingTileBound;
        private static Dictionary<int, Rectangle> _bounds;
        private static HashSet<int> _boundsID;
        private static float Time = 0f;

        static bool  isDebug = false;
        static bool showHitbox = false;
        static bool showGameTime = false;
        static PlayerStatus status = PlayerStatus.Idle;
        static private Vector2 pPosition;
        static Vector2 velocity;

        static int spacing = 0;
        public static void Load(ContentManager content)
        {
            spriteFont = content.Load<SpriteFont>("Font/text");
            squareTexture = content.Load<Texture2D>("hitbox_square64");
            circleTexture = content.Load<Texture2D>("hitbox_circle64");
            _boundsID = new HashSet<int>();
            _bounds = new Dictionary<int, Rectangle>();
        }

        public static void Update(GameTime gameTime, List<Circle> circles, List<FallingTile> fallingTile)
        {
            float delta = (float)gameTime.TotalGameTime.TotalSeconds;
            itemBound = circles;
            fallingTileBound = fallingTile;

            if (InputManager.Input(Keys.F1))
            {
                isDebug = !isDebug;
                Console.WriteLine(isDebug);
            }

            if (isDebug && InputManager.Input(Keys.F2))
            {
                showHitbox = !showHitbox;
            }

            if (isDebug && InputManager.Input(Keys.F3))
            {
                showGameTime = !showGameTime;
            }

            Time = delta;
        }

        public static void AddToDebugBound(Rectangle bound, int id)
        {
            if (_boundsID.Contains(id))
            {
                _boundsID.Add(id);
                _bounds.Add(id, bound);
                return;
            }
            else
            {
                _bounds[id] = bound;
            }
        }
        public static void GetPlayerPosition(Vector2 playerPosition)
        {
            pPosition = playerPosition;
        }
        public static void DrawDebugRectangle(SpriteBatch spriteBatch)
        {
            int i = 0;
            if (showHitbox && isDebug)
            {
                foreach (var rect in _bounds.Values)
                {
                    spriteBatch.Draw(squareTexture, rect, Color.LightGreen);
                    i++;
                }

                foreach (var rect in fallingTileBound)
                {
                    spriteBatch.Draw(squareTexture, rect.BoundingRec, Color.Red);
                }
                DrawItemBound(spriteBatch);
            }
        }

        public static void DrawFallingTile(SpriteBatch sb)
        {

        }

        public static void DrawItemBound(SpriteBatch sb)
        {
            if (showHitbox && isDebug)
            foreach (var circle in itemBound)
            {
                float scaleFactor = (circle.Radius * 2) / 64f;
                Vector2 origin = new Vector2(64 / 2, 64 / 2);
                sb.Draw(circleTexture, new Vector2(circle.Center.X, circle.Center.Y),
                    new Rectangle(0,0, 64, 64), Color.DarkRed, 0f, origin, scaleFactor, SpriteEffects.None, 0f);
            }
        }

        public static void GetCurrentAction(PlayerStatus playerStatus)
        {
            status = playerStatus;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spacing = 0;
            spriteBatch.Begin();
            if (isDebug)
            {
                DrawText(spriteBatch, $"Game Time {Time}", 0);
                DrawText(spriteBatch, $"Show Hitbox F1 - {showHitbox}", 1);
                DrawText(spriteBatch, $"Player action - {status}", 2);
                DrawText(spriteBatch, $"Player position - {pPosition}", 3);
                DrawText(spriteBatch, $"Mouse Position - {InputManager.GetMousePosition()}", 3);
                DrawText(spriteBatch, $"Player Velocity - {velocity}", 4);
            }

            spriteBatch.End();
        }

        public static void GetVelocity(Vector2 playerVelocity)
        {
            velocity = playerVelocity;
        }
        private static void DrawText(SpriteBatch spriteBatch, string txt, int id)
        {
            spriteBatch.DrawString(spriteFont, txt, new Vector2(0, spacing), Color.White);
            spacing += 20;
        }
    }
#endif
}
