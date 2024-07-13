using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallen_Knight.src.Core
{
    public class Canvas
    {
        private readonly RenderTarget2D _target;
        private readonly GraphicsDevice _device;
        private Rectangle _destinationRectangle;

        public Canvas(GraphicsDevice graphicsDevice, int width, int height)
        {
            _device = graphicsDevice;
            _target = new(_device, width, height);
            SetDestinationRectangle();
        }

        public void SetDestinationRectangle()
        {
            var screenSize = _device.PresentationParameters.Bounds;

            float scaleX = (float)screenSize.Width / _target.Width;
            float scaleY = (float)screenSize.Height / _target.Height;
            float scale = Math.Min(scaleX, scaleY);

            int newWidth = (int)(_target.Width * scale);
            int newHeight = (int)(_target.Height * scale);

            int posX = (screenSize.Width - newWidth) / 2;
            int posY = (screenSize.Height - newHeight) / 2;

            _destinationRectangle = new Rectangle(posX, posY, newWidth, newHeight);
        }

        public void Activate()
        {
            _device.SetRenderTarget(_target);
            _device.Clear(Color.DarkGray);
        }

        public void Draw(SpriteBatch sprite)
        {
            _device.SetRenderTarget(null);
            _device.Clear(Color.Black);
            sprite.Begin();
            sprite.Draw(_target, _destinationRectangle, Color.White);
            sprite.End();

        }

    }
}
