using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.GameAssets.Interface;
using Fallen_Knight.GameAssets.Items;
using Fallen_Knight.GameAssets.Mobs;
using Fallen_Knight.GameAssets.Tile.Tile;
using Fallen_Knight.GameAssets.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Fallen_Knight.GameAssets.Levels
{
    public class Level
    {
        public Tiles.Tile[,] tiles;
        public IGameEntity Player;
        public IGameEntity Enemy;
        public List<BonusItem> ItemBonus;
        public List<FallingTile> FallingTiles;
        public Dictionary<Rectangle, (TileType, char)> tileMap;
        private Vector2 playerSpawn;
        private Texture2D goldBag;

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
        public void Load(IServiceProvider content, GraphicsDevice graphicsDevice)
        {
            contentManager = new ContentManager(content, "Content");
            this.graphicsDevice = graphicsDevice;
        }

        public void Update(GameTime gameTime)
        {
            Player.Update(gameTime);
            Enemy.Update(gameTime);
            Player player = (Player)Player;
            foreach (var item in ItemBonus)
            {
                item.Update(gameTime, player);
            }
            UpateFallingTile(gameTime);
            CollectItem();
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

                        if (token == "-1")
                        {
                            sb.Append('!');
                        }
                        else
                        {
                            sb.Append(token);
                        }

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

                    Rectangle tileRect = new Rectangle(j * (int)Tiles.Tile.Size.X, i * (int)Tiles.Tile.Size.Y, 64, 65);

                    if (!tileMap.ContainsKey(tileRect))
                    {
                        tileMap.Add(tileRect, (tileType, tiletypes));
                    }

                    LoadEntities(tileType, i, j);

                    SetTile(tileType, i, j, tiletypes);
                }
            }
        }

        public void LoadEntities(TileType tileType, int y, int x)
        {
            switch (tileType)
            {
                case TileType.Spawn:
                    playerSpawn = new Vector2(x * Tiles.Tile.Size.Y, y * Tiles.Tile.Size.Y);
                    Player player = (Player)Player;
                    player.SetPlayerSpawn(playerSpawn);
                    break;
                case TileType.Item:
                    if (ItemBonus == null)
                        ItemBonus = new List<BonusItem>();
                    ItemBonus.Add(new BonusItem(new Vector2(x * Tiles.Tile.Size.Y, y * Tiles.Tile.Size.Y + 32), goldBag, this));
                    break;
                case TileType.FallingPlatform:
                    if (FallingTiles == null)
                        FallingTiles = new List<FallingTile>();
                    Texture2D t = Content.Load<Texture2D>("Tiles/hitbox_square64");
                    FallingTiles.Add(new FallingTile(t, new Vector2(x * Tiles.Tile.Size.Y, y * Tiles.Tile.Size.Y), this));
                    break;
            }
        }

        public TileType LoadVarietyObject(char token)
        {
            switch (token)
            {
                case '?':
                    return TileType.Impassable;
                case '%':
                    return TileType.Spawn;
                case '!':
                    return TileType.Passable;
                case '9':
                    return TileType.Item;
                case 'f':
                    return TileType.FallingPlatform;
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
                case '=':
                    return TileType.Platform;
                case '-':
                    return TileType.Platform;

                default:
                    return TileType.Platform;
            }
        }

        public void LoadGameEntities()
        {
            Player = new Player(this, Content , graphicsDevice);
            Enemy = new Enemy(Content.Load<Texture2D>("Monster/slimemonster"), new Vector2(100, 600), this);
            goldBag = Content.Load<Texture2D>("Item/goldbag");
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
            Player.Draw(spriteBatch, gameTime);
            Enemy.Draw(spriteBatch, gameTime);

            foreach (var items in ItemBonus)
            {
                items.Draw(spriteBatch);
            }

            DrawFallingTile(spriteBatch, gameTime);

        }

        private void DrawFallingTile(SpriteBatch sb, GameTime gameTime)
        {
            foreach (var item in FallingTiles)
            {
                item.Draw(sb, gameTime);
            }
        }

        private void UpateFallingTile(GameTime gameTime)
        {
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
                        spriteBatch.Draw(tiles[x, y].Texture, new Rectangle(x * 64, y * 65, 64, 65),
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
                '0' => "Tiles/t0",
                '1' => "Tiles/t",
                '2' => "Tiles/t1",
                '3' => "Tiles/t2",
                '4' => "Tiles/t3",
                '5' => "Tiles/t4",
                '6' => "Tiles/t5",
                '7' => "Tiles/t6",
                '8' => "Tiles/t7",
                '=' => "Tiles/t8",
                '-' => "Tiles/t9",
                '9' => "Item/goldbag",
                'f' => "Tiles/hitbox_square64",
                _ => throw new InvalidOperationException()

            };

            return Content.Load<Texture2D>(texture);
        }

    }
}
