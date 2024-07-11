using Fallen_Knight.GameAssets.Camera;
using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.GameAssets.Collisions;
using Fallen_Knight.GameAssets.Layers;
using Fallen_Knight.GameAssets.Levels;
using Fallen_Knight.GameAssets.Mobs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace Fallen_Knight.src.Core
{
    public class GameManager
    {

        private readonly Level level;
        private readonly Layer layer;
        private readonly Game _game;
        private ContentManager content;
        Point screenSize = new Point(1280, 720);
        private Texture2D background;
        private DebugHelper debugHelper;

        float scrollSpeedMax = 300f;
        float scrollSpeed;

        private readonly GraphicsDeviceManager _graphics;

        private Camera camera;
        const int GAME_WORLD_WIDTH = 8640;
        const int GAME_WORLD_HEIGHT = 720;
        const int SCROLL_RIGHT = -1;
        const int SCROLL_LEFT = 1;
        public GameManager(Game game, GraphicsDeviceManager deviceManager)
        {
            _game = game;
            _graphics = deviceManager;

            _graphics.PreferredBackBufferWidth = screenSize.X;
            _graphics.PreferredBackBufferHeight = screenSize.Y;
            level = new Level(screenSize);
            layer = new Layer();

        }

        public void LoadContent(ContentManager content)
        {

            camera = new Camera(_graphics.GraphicsDevice.Viewport);
            debugHelper = new DebugHelper();
            debugHelper.Load(content);
            background = content.Load<Texture2D>("Scene/back");
            level.Load(content.ServiceProvider, _graphics.GraphicsDevice);
            string filePath = "Content/Levels/tile.csv";
            FileStream fs = File.OpenRead(filePath);
            level.LoadTile(fs);
            layer.Load(content, new Vector2(screenSize.X, screenSize.Y), level);
        }

        private void SetResolution(int width, int height)
        {
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();
        }
        public void Update(GameTime gameTime)
        {
            InputManager.Update();
            if (InputManager.Input(Keys.F5)) SetResolution(400, 300);
            if (InputManager.Input(Keys.F6)) SetResolution(1920, 1080);
            if (InputManager.Input(Keys.F7)) SetResolution(640, 1080);
            if (InputManager.Input(Keys.F8)) SetResolution(1280, 720);

            Player player = (Player)level.Player;
            Enemy enemy = (Enemy)level.Enemy;


            List<Rectangle> target = new List<Rectangle>
            {
                player.Hitbox[0],
                player.Hitbox[1],
                player.Hitbox[2],
                player.Hitbox[3],
                enemy.EnemyBound
            };

            List<Circle> circles = new List<Circle>();
            foreach (var item in level.ItemBonus)
            {
                circles.Add(item.GetItemBound());
            }

            debugHelper.Update(gameTime, Keyboard.GetState(), target, circles);
            debugHelper.GetCurrentAction(player.CurrentAction);
            level.Update(gameTime);
            camera.Update(gameTime, player, new Rectangle(0, 0, GAME_WORLD_WIDTH, GAME_WORLD_HEIGHT));
        }

        public void Draw(SpriteBatch spriteBatch , GameTime gameTime)
        {
            layer.BackgroundDraw(gameTime, _graphics.GraphicsDevice, spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);

            level.Draw(spriteBatch, gameTime);
            debugHelper.DrawDebugRectangle(spriteBatch);
            spriteBatch.End();
            debugHelper.Draw(spriteBatch);
        }
    }
}
