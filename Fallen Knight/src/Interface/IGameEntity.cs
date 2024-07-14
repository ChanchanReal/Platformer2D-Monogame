using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallen_Knight.GameAssets.Interface
{
    public interface IGameEntity
    {
        public void Update(GameTime gameTime);
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
