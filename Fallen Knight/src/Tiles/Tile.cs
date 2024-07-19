using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fallen_Knight.GameAssets.Tiles
{
    public enum TileType
    {
        None = 0,

        Spawn = 1,

        Passable = 2,

        Impassable = 3,

        Platform = 4,

        FallingPlatform = 5,

        Exit = 6,

        Item = 9,

        Enemy = 10,
    }
    public struct Tile
    {
        public Texture2D Texture;
        public TileType Tile_Type;

        public const int width = 64;
        public const int height = 64;

        public static Vector2 Size => new Vector2(width, height);
        public Tile(Texture2D texture, TileType tileType)
        {
            Texture = texture;
            Tile_Type = tileType;
        }

        public void Update()
        {

        }
    }
}
