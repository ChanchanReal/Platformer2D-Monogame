using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fallen_Knight.GameAssets.Interface
{
    public interface IGameEntity
    {
        public void Update(GameTime gameTime);
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
