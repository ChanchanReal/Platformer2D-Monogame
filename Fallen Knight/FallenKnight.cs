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
using System.IO;

namespace Fallen_Knight
{
    public class FallenKnight : Microsoft.Xna.Framework.Game
    {
        DebugHelper debugHelper;

        private Level level;
        private Layer layer;
        private Texture2D background;
        Point screenSize = new Point(1280, 720);
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Camera camera;
        const int GAME_WORLD_WIDTH = 8640;
        const int GAME_WORLD_HEIGHT = 720;
        const int SCROLL_RIGHT = -1;
        const int SCROLL_LEFT = 1;

        float scrollSpeedMax = 300f;
        float scrollSpeed;

        KeyboardState kb;

        public FallenKnight()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = screenSize.X;
            _graphics.PreferredBackBufferHeight = screenSize.Y;
            level = new Level(screenSize);
            layer = new Layer();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            camera = new Camera(GraphicsDevice.Viewport);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            debugHelper = new DebugHelper();
            debugHelper.Load(Content);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("Scene/back");
            level.Load(Content.ServiceProvider, GraphicsDevice);
            string filePath = "Content/Levels/tile.csv";
            FileStream fs = File.OpenRead(filePath);
            level.LoadTile(fs);
            layer.Load(Content, new Vector2(screenSize.X, screenSize.Y), level);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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
            // TODO: Add your update logic here
            level.Update(gameTime);
            kb = Keyboard.GetState();
            camera.Update(gameTime, player, new Rectangle(0,0, GAME_WORLD_WIDTH, GAME_WORLD_HEIGHT));
            base.Update(gameTime);
        }

        

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            layer.BackgroundDraw(gameTime, GraphicsDevice, _spriteBatch);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null , null ,null, camera.transform);

            level.Draw(_spriteBatch, gameTime);
            debugHelper.DrawDebugRectangle(_spriteBatch);
            _spriteBatch.End();
            debugHelper.Draw(_spriteBatch);
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
