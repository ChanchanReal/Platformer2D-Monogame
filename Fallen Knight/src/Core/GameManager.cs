using Fallen_Knight.GameAssets;
using Fallen_Knight.GameAssets.Camera;
using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.GameAssets.Collisions;
using Fallen_Knight.GameAssets.Layers;
using Fallen_Knight.GameAssets.Levels;
using Fallen_Knight.GameAssets.Mobs;
using Fallen_Knight.src.Score;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fallen_Knight.src.Core
{
    public class GameManager
    {
        private readonly GameSoundManager gameSound;
        private Level level;
        private readonly Layer layer;
        private readonly Game _game;
        private Canvas _canvas;
        private ContentManager content;
        private Texture2D background;
        private SpriteFont font;
        Point screenSize = new Point(1280, 720);
        float scrollSpeedMax = 300f;
        float scrollSpeed;

        List<Texture2D> ParticleTexture;

        private readonly GraphicsDeviceManager _graphics;

        private Camera camera;
        const int GAME_WORLD_WIDTH = 8640;
        const int GAME_WORLD_HEIGHT = 920;
        const int SCROLL_RIGHT = -1;
        const int SCROLL_LEFT = 1;
        public GameManager(Game game, GraphicsDeviceManager deviceManager, ContentManager content)
        {
            _game = game;
            _graphics = deviceManager;
            _graphics.PreferredBackBufferWidth = screenSize.X;
            _graphics.PreferredBackBufferHeight = screenSize.Y;
            level = new Level(new Point(GAME_WORLD_WIDTH, GAME_WORLD_HEIGHT));
            layer = new Layer();
            gameSound = new GameSoundManager();
            this.content = content;
            
        }

        public void LoadContent()
        {

            camera = new Camera(_graphics.GraphicsDevice.Viewport);
            background = content.Load<Texture2D>("Scene/back");
            _canvas = new(_graphics.GraphicsDevice, 1280, 720);

            ParticleTexture = new List<Texture2D>();
            ParticleTexture.Add(content.Load<Texture2D>("Particles/particle"));

            level.Load(content.ServiceProvider,
                _graphics.GraphicsDevice,
                new ParticleSystem(Vector2.Zero, ParticleTexture), gameSound);

            string filePath = "Content/Levels/falling_map.csv";
            FileStream fs = File.OpenRead(filePath);
            level.LoadTile(fs);
            layer.Load(content, _graphics.GraphicsDevice, new Vector2(screenSize.X, screenSize.Y), level);
            gameSound.LoadSounds(content);
            gameSound.PlayBackgroundMusic();
            font = content.Load<SpriteFont>("Font/text");
        }

        private void SetResolution(int width, int height)
        {
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();
            _canvas.SetDestinationRectangle();
        }
        public void Update(GameTime gameTime)
        {

            if (level.ExitReached)
            {
                SwitchNewLevel();
            }

            InputManager.Update();
            if (InputManager.Input(Keys.F5)) SetResolution(400, 300);
            if (InputManager.Input(Keys.F6)) SetResolution(1920, 1080);
            if (InputManager.Input(Keys.F7)) SetResolution(640, 1080);
            if (InputManager.Input(Keys.F8)) SetResolution(1280, 720);

            Player player = (Player)level.Player;

            if (level.enemies.Count > 0)
            {
                Enemy enemy = (Enemy)level.enemies.First();
            }
#if DEBUG

            List<Circle> circles = new List<Circle>();
            foreach (var item in level.ItemBonus)
            {
                circles.Add(item.GetItemBound());
            }
            DebugHelper.Update(gameTime, circles, level.FallingTiles);

#endif
            level.Update(gameTime);
            layer.Update(gameTime);
            camera.Update(gameTime, player, new Rectangle(0, 0, GAME_WORLD_WIDTH, GAME_WORLD_HEIGHT));


            InputManager.SetMousePosition(camera.ScreenToWorld(InputManager.GetMousePositionFromCamera()));
        }

        private void SwitchNewLevel()
        {
            level = new Level(screenSize);

            string filePath = "Content/Levels/falling_map2.csv";
            FileStream fs = File.OpenRead(filePath);
            level.Load(content.ServiceProvider,
               _graphics.GraphicsDevice,
               new ParticleSystem(Vector2.Zero, ParticleTexture), gameSound);
            level.LoadTile(fs);
            layer.Load(content, _graphics.GraphicsDevice, new Vector2(screenSize.X, screenSize.Y), level);
        }

        public void Draw(SpriteBatch spriteBatch , GameTime gameTime)
        {
            _canvas.Activate();
            _canvas.Draw(spriteBatch);
            layer.BackgroundDraw(gameTime, spriteBatch);
            level.DrawPlayerEffect(spriteBatch, gameTime, camera.transform);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);
            level.Draw(spriteBatch, gameTime);
            DebugHelper.DrawDebugRectangle(spriteBatch);
            DebugHelper.DrawItemBound(spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Total Score  " + Score.Score.GetScore().ToString(), new Vector2(0, 10), Color.White);
            spriteBatch.End();

        }
    }
}
