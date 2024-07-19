using Fallen_Knight.GameAssets.Animations;
using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.GameAssets.Interface;
using Fallen_Knight.GameAssets.Items;
using Fallen_Knight.GameAssets.Mobs;
using Fallen_Knight.GameAssets.Tiles;
using Fallen_Knight.src.Core;
using Fallen_Knight.src.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Fallen_Knight.GameAssets.Levels
{
    public class Level : IDisposable
    {
        public Tiles.Tile[,] tiles;
        public IGameEntity Player;
        public List<IGameEntity> enemies;
        public List<BonusItem> ItemBonus;
        public List<FallingTile> FallingTiles;
        public ExitTile ExitTile;
        private ParticleSystem particleSys;
        private Animation spawnAnimation;
        public Dictionary<Rectangle, (TileType, char)> tileMap;
        private Vector2 playerSpawn;
        private Texture2D goldBag;
        private GameSoundManager gameSound;
        public bool ExitReached { get; set; } = false;

        public ContentManager Content
        {
            get => contentManager;
        }
        ContentManager contentManager;
        public Point WorldSize
        {
            get => worldSize;
        }
        Point worldSize;
        public Level(Point screenSize)
        {
            this.worldSize = screenSize;
        }

        private GraphicsDevice graphicsDevice;
        public void Load(IServiceProvider content, GraphicsDevice graphicsDevice,
            ParticleSystem particleSys , GameSoundManager gameSound)
        {
            this.gameSound = gameSound;
            contentManager = new ContentManager(content, "Content");
            this.graphicsDevice = graphicsDevice;
            this.particleSys = particleSys;
        }

        public void Update(GameTime gameTime)
        {
            Player.Update(gameTime);
            spawnAnimation.UpdateFrame(gameTime);
            UpdateEnemy(gameTime);

            Player player = (Player)Player;

            foreach (var item in ItemBonus)
            {
                item.Update(gameTime, player);
            }
            UpateFallingTile(gameTime);
            CollectItem();
            ExitTile?.Update(gameTime);
        }

        public void UpdateEnemy(GameTime gameTime)
        {
            foreach (var enemy in enemies)
            {
                enemy.Update(gameTime);
            }

            IEnumerable<Enemy> aliveEnemies = enemies
                                  .OfType<Enemy>()
                                  .Where(enemy => enemy.IsAlive);

            enemies = aliveEnemies.ToList<IGameEntity>();
        }

        public void CollectItem()
        {
            List<BonusItem> updateItem = new List<BonusItem>();

            foreach (var item in ItemBonus)
            {
                if (item.IsCollected)
                {
                    updateItem.Add(item);
                }
            }

            foreach (var item in updateItem)
            {
                ItemBonus.Remove(item);
            }
        }
        public void LoadTile(FileStream fileStream)
        {
            LoadGameEntities();

            int width = 0;

            List<string> lines = new List<string>();
            StringBuilder sb = new StringBuilder();

            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;

                while (line != null)
                {
                    string[] tokens = line.Split(',');

                    foreach (var token in tokens)
                    {
                        sb.Append(TranslateChar(token));
                    }

                    lines.Add(sb.ToString());
                    width = sb.Length;

                    if (sb.Length != width)
                    {
                        throw new Exception("Map is not initialized properly");
                    }

                    sb = new StringBuilder();
                    line = reader.ReadLine();
                }
            }

            int height = lines.Count;
            tileMap = new Dictionary<Rectangle, (TileType, char)>();
            tiles = new Tiles.Tile[width, height];

            for (int i = 0; i < Height; ++i)
            {
                for (int j = 0; j < Width; ++j)
                {
                    char tiletypes = lines[i][j];

                    TileType tileType = LoadVarietyObject(tiletypes);

                    Rectangle tileRect = new Rectangle(j * (int)Tiles.Tile.Size.X, i * (int)Tiles.Tile.Size.Y, 64, 64);

                    if (!tileMap.ContainsKey(tileRect))
                    {
                        tileMap.Add(tileRect, (tileType, tiletypes));
                    }

                    LoadEntities(tileType, i, j, tiletypes);
                }
            }
        }

        private string TranslateChar(string num)
        {
            return num switch
            {
                "-1" => "!",
                "10" => "%",
                "11" => "e",
                "12" => "$",
                "13" => "f",
                "14" => "~",
                _    => num,
            };
        }

        public void LoadEntities(TileType tileType, int y, int x, char type)
        {
            switch (tileType)
            {
                case TileType.Exit:
                    SetExitTile(x, y);
                    break;
                case TileType.Platform:
                    SetTile(tileType, y, x, type);
                    break;
                case TileType.Spawn:
                    SetSpawn(x, y);
                    break;
                case TileType.Item:
                    SetItem(x, y);
                    break;
                case TileType.FallingPlatform:
                    SetFallingPlatForm(x, y);
                    break;

                case TileType.Enemy:
                    SetEnemy(x, y);
                    break;

            }
        }

        public void SetEnemy(int x, int y)
        {
            enemies.Add(
                new RobeEnemy(Content.Load<Texture2D>("Monster/executionair-Sheet"),
                this,
                new Vector2(x * Tiles.Tile.Size.Y, y * Tiles.Tile.Size.Y))
                );
        }

        public void SetExitTile(int x, int y)
        {
            ExitTile = new ExitTile(this,
                Content.Load<Texture2D>("Tiles/teleporter"),
                new Rectangle((int)(x * Tiles.Tile.Size.Y),
                (int)(y * Tiles.Tile.Size.Y), 64, 64)
                );
        }

        public void SetFallingPlatForm(int x, int y)
        {
            if (FallingTiles == null)
                FallingTiles = new List<FallingTile>();

            Texture2D t = Content.Load<Texture2D>("Tiles/fallingTile");
            FallingTiles.Add(new FallingTile(t, new Vector2(x * Tiles.Tile.Size.Y, y * Tiles.Tile.Size.Y), this, gameSound));
        }

        public void SetItem(int x, int y)
        {
            if (ItemBonus == null)
                ItemBonus = new List<BonusItem>();
            ItemBonus.Add(new BonusItem(new Vector2(x * Tiles.Tile.Size.Y + 32, y * Tiles.Tile.Size.Y + 32), goldBag, this));
        }

        public void SetSpawn(int x, int y)
        {
            playerSpawn = new Vector2(x * Tiles.Tile.Size.Y, y * Tiles.Tile.Size.Y);
            Player player = (Player)Player;
            player.SetPlayerSpawn(playerSpawn);
            spawnAnimation = new Animation(Content.Load<Texture2D>("Tiles/spawn"), 64, 64);
            spawnAnimation.Position = new Rectangle((int)(x * Tiles.Tile.Size.Y),
                (int)(y * Tiles.Tile.Size.Y), 64, 64);
        }

        public TileType LoadVarietyObject(char token)
        {
            switch (token)
            {
                case 'e':
                    return TileType.Exit;
                case '?':
                    return TileType.Impassable;
                case '%':
                    return TileType.Spawn;
                case '~':
                    return TileType.Enemy;
                case '!':
                    return TileType.Passable;
                case '$':
                    return TileType.Item;
                case 'f':
                    return TileType.FallingPlatform;
                case '0':
                    return TileType.Platform;
                case '1':
                    return TileType.Platform;
                case '2':
                    return TileType.Platform;
                case '3':
                    return TileType.Platform;
                case '4':
                    return TileType.Platform;
                case '5':
                    return TileType.Platform;
                case '6':
                    return TileType.Platform;
                case '7':
                    return TileType.Platform;
                case '8':
                    return TileType.Platform;
                case '9':
                    return TileType.Platform;

                default:
                    return TileType.Platform;
            }
        }

        public void LoadGameEntities()
        {
            Player = new Player(this, Content , graphicsDevice, particleSys);
            goldBag = Content.Load<Texture2D>("Item/goldbag");
            enemies = new List<IGameEntity>();
        }

        public TileType GetCollision(int x, int y)
        {
            // limit it to the sky cant jump further
            if (x < 0 || x >= Width)
                return TileType.Impassable;

            // limit to side
            if (y < 0 || y >= Height)
                return TileType.Passable;

            return tiles[x, y].Tile_Type;
        }
        // get the Tile bounding in the level
        public Rectangle GetBounds(int x, int y)
        {
            // get the size of the tile
            return new Rectangle(x * (int)Tiles.Tile.Size.X, y * (int)Tiles.Tile.Size.Y,
                (int)Tiles.Tile.Size.X, (int)Tiles.Tile.Size.X);
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawTile(spriteBatch);
            spawnAnimation.Draw(spriteBatch);
            Player.Draw(spriteBatch, gameTime);
            foreach (var enemy in enemies)
            {
                enemy.Draw(spriteBatch, gameTime);
            }
            foreach (var items in ItemBonus)
            {
                items.Draw(spriteBatch);
            }
            DrawFallingTile(spriteBatch, gameTime);
            ExitTile?.Draw(spriteBatch);
        }

        public void DrawPlayerEffect(SpriteBatch spriteBatch, GameTime gameTime, Matrix camera)
        {
            Player player = (Player)Player;
            player.DrawPlayerEffect(spriteBatch, gameTime, camera);
        }

        private void DrawFallingTile(SpriteBatch sb, GameTime gameTime)
        {
            if (FallingTiles != null)
            foreach (var item in FallingTiles)
            {
                item.Draw(sb, gameTime);
            }
        }

        private void UpateFallingTile(GameTime gameTime)
        {
            if (FallingTiles != null)
            foreach (var item in FallingTiles)
            {
                item.Update(gameTime);
            }
        }

        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        private void DrawTile(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    if (tiles[x, y].Tile_Type == TileType.Platform)
                    {
                        spriteBatch.Draw(tiles[x, y].Texture, new Rectangle(x * 64, y * 64, 64, 64),
                            new Rectangle(0, 0, 64, 64), Color.White);
                    }
                }
            }
        }
        private void SetTile(TileType tileType, int x, int y, char token)
        {
            if (token == '%' || token == '!')
                return;

            tiles[y, x] = new Tiles.Tile(LoadTileVarietyTexture(token), tileType);
        }

        public Texture2D LoadTileVarietyTexture(char token)
        {
            string texture = token switch
            {
                '0' => "Tiles/darktile/base",
                '1' => "Tiles/darktile/black",
                '2' => "Tiles/darktile/t1",
                '3' => "Tiles/darktile/t2",
                '4' => "Tiles/darktile/t3",
                '5' => "Tiles/darktile/t4",
                '6' => "Tiles/darktile/t5",
                '7' => "Tiles/darktile/t6",
                '8' => "Tiles/darktile/t7",
                '9' => "Tiles/darktile/t8",
                '$' => "Item/goldbag",
                'f' => "Tiles/hitbox_square64",
                '~' => "Monster/executionair-Sheet",
                _ => throw new InvalidOperationException()

            };

            return Content.Load<Texture2D>(texture);
        }

        public void Dispose()
        {
            Content.Dispose();
        }
    }
}
