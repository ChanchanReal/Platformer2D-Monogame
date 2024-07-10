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
        public Point ScreenSize
        {
            get => screenSize;
        }
        Point screenSize;
        public Level(Point screenSize)
        {
            this.screenSize = screenSize;
        }

        public void Load(IServiceProvider content, GraphicsDevice graphicsDevice)
        {
            contentManager = new ContentManager(content, "Content");
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

            width = 0;
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
                            sb.Append('!');
                        else
                            sb.Append(token);
                    }
                    lines.Add(sb.ToString());
                    width = sb.Length;
                    if (sb.Length != width)
                        throw new Exception("Map is not initialized properly");
                    sb = new StringBuilder();
                    line = reader.ReadLine();
                }
            }
            height = lines.Count;

            tileMap = new Dictionary<Rectangle, (TileType, char)>();

            tiles = new Tiles.Tile[Width, Height];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
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

                default:
                    return TileType.Platform;
            }
        }

        public void LoadGameEntities()
        {
            Player = new Player(this, Content);
            Enemy = new Enemy(Content.Load<Texture2D>("Monster/slimemonster"), new Vector2(100, 600), this);
            goldBag = Content.Load<Texture2D>("Item/goldbag");
        }

        public TileType GetCollision(int x, int y)
        {
            // limit it to the sky cant jump further
            if (x < 0 || x >= ScreenSize.Y)
                return TileType.Impassable;
            // limit to side
            if (y < 0 || y >= ScreenSize.X)
                return TileType.Passable;

            x = Math.Clamp(x, 0, tiles.GetLength(0) - 1);
            y = Math.Clamp(y, 0, tiles.GetLength(1) - 1);

            return tiles[x, y].Tile_Type;
        }
        // get the Tile bounding in the level
        public Rectangle GetBounds(int x, int y)
        {
            // get the size of the tile
            return new Rectangle(x * (int)Tiles.Tile.Size.X, y * (int)Tiles.Tile.Size.Y, (int)Tiles.Tile.Size.X, (int)Tiles.Tile.Size.X);
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawTile(spriteBatch);
            Player.Draw(spriteBatch);
            Enemy.Draw(spriteBatch);

            foreach (var items in ItemBonus)
            {
                items.Draw(spriteBatch);
            }

            DrawFallingTile(spriteBatch);

        }

        private void DrawFallingTile(SpriteBatch sb)
        {
            foreach (var item in FallingTiles)
            {
                item.Draw(sb);
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
            get { return width; }
        }
        private int width;

        public int Height
        {
            get { return height; }
        }
        private int height;

        private void DrawTile(SpriteBatch sprite)
        {

            foreach (var Tile in tileMap.Keys)
            {
                if (tileMap[Tile].Item1 == TileType.Platform)
                {
                    int newX = Tile.X / 64;
                    int newY = Tile.Y / 65;

                    sprite.Draw(LoadTileVarietyTexture(tileMap[Tile].Item2), new Rectangle(Tile.X, Tile.Y, 64, 65),
                        new Rectangle(0, 0, 64, 64), Color.AliceBlue);
                }
            }
        }
        private void SetTile(TileType tileType, int x, int y, char token)
        {
            x = Math.Clamp(x, 0, Width);
            y = Math.Clamp(y, 0, Height - 1);

            if (token != '%' && token != '!')
                tiles[x, y] = new Tiles.Tile(LoadTileVarietyTexture(token), tileType);
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
