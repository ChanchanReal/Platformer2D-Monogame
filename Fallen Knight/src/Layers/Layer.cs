using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.GameAssets.Levels;
using Fallen_Knight.src.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Formats.Asn1.AsnWriter;

namespace Fallen_Knight.GameAssets.Layers
{
    public class Layer
    {
        private Texture2D _staticBackground;
        // close layer
        private Texture2D _closeLayer;
        private Rectangle _closeLayerRect;
        private Vector2 _originalPositionMirrorCloseLayer;
        // mid layer
        private Texture2D _midLayer;
        private Rectangle _midLayerRect;
        private Vector2 _midLayerPos;
        // far layer
        private Texture2D _farLayer;
        private Rectangle _farLayerRect;
        private Vector2 _farLayerPos;

        // mirrors
        // close layer
        private Rectangle _closeLayerMirrorRect;
        private Vector2 _originalPositionCloseLayer;
        // mid layer
        private Rectangle _midMirrorLayerRect;
        private Vector2 _midMirrorLayerPos;
        // far layer
        private Rectangle _farMirrorLayerRect;
        private Vector2 _farMirrorLayerPos;


        private Vector2 _screenSize;
        private Level _level;

        float scrollSpeedMax_Close = 10.5f; 
        float scrollSpeedMax_Mid = 7.75f; 
        float scrollSpeedMax_Far = 4.4f; 

        public void Load(ContentManager contentManager, GraphicsDevice graphicsDevice,
            Vector2 screenSize, Level level)
        {
            this._screenSize = screenSize;
            this._level = level;
            // that doesnt change
            _staticBackground = ReScaleImage(graphicsDevice, 
                contentManager.Load<Texture2D>("Scene/parallax-demon-woods-bg"),
                1280, 720);
            // the one that is closest to player
            _closeLayer = ReScaleImage( graphicsDevice,
                contentManager.Load<Texture2D>("Scene/parallax-demon-woods-close-trees"),
                1280, 720);

            // little slower than the close layer
            _midLayer = ReScaleImage(graphicsDevice,
                contentManager.Load<Texture2D>("Scene/parallax-demon-woods-mid-trees"),
                1280, 720);

            // slower than mid
            _farLayer = ReScaleImage(graphicsDevice,
    contentManager.Load<Texture2D>("Scene/parallax-demon-woods-far-trees"),
    1280, 720);

            _closeLayerRect = new Rectangle(0, 0, 1280, 720);
            _closeLayerMirrorRect = new Rectangle((int)_screenSize.X, 0, 1280, 720);
            _originalPositionCloseLayer = new Vector2(_closeLayerRect.X, _closeLayerRect.Y);
            _originalPositionMirrorCloseLayer = new Vector2(_closeLayerMirrorRect.X, _closeLayerMirrorRect.Y);

            // mid
            _midLayerRect = new Rectangle(0, 0, 1280, 720);
            _midMirrorLayerRect = new Rectangle((int)_screenSize.X, 0, 1280, 720);
            _midLayerPos = new Vector2(_midLayerRect.X, _midLayerRect.Y);
            _midMirrorLayerPos = new Vector2(_midMirrorLayerRect.X, _midMirrorLayerRect.Y);

            // far
            _farLayerRect = new Rectangle(0, 0, 1280, 720);
            _farMirrorLayerRect = new Rectangle((int)_screenSize.X, 0, 1280, 720);
            _farLayerPos = new Vector2(_farLayerRect.X, _farLayerRect.Y);
            _farMirrorLayerPos = new Vector2(_farMirrorLayerRect.X, _farMirrorLayerRect.Y);


        }

        private Texture2D ReScaleImage(GraphicsDevice graphicsDevice, Texture2D originalTexture, int newWidth, int newHeight)
        {
            RenderTarget2D renderTarget = new RenderTarget2D(graphicsDevice, newWidth, newHeight);

            graphicsDevice.SetRenderTarget(renderTarget);
            graphicsDevice.Clear(Color.Transparent);

            SpriteBatch spriteBatch = new SpriteBatch(graphicsDevice);
            spriteBatch.Begin();

            spriteBatch.Draw(originalTexture, new Rectangle(0, 0, newWidth, newHeight), Color.White);
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);

            return renderTarget;
        }

        public void Update(GameTime gameTime)
        {
           float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdatePosition(delta);
        }

        private void UpdatePosition(float delta)
        {
            Player player = (Player)_level.Player;
            float playerSpeedX = player.PlayerSpeed.X;

            float scrollSpeedClose = playerSpeedX * scrollSpeedMax_Close * delta;
            float scrollSpeedMid = playerSpeedX * scrollSpeedMax_Mid * delta;
            float scrollSpeedFar = playerSpeedX * scrollSpeedMax_Far * delta;

            // Move the layers in the opposite direction of the player
            _originalPositionCloseLayer.X -= scrollSpeedClose;
            _originalPositionMirrorCloseLayer.X -= scrollSpeedClose;

            _midLayerPos.X -= scrollSpeedMid;
            _midMirrorLayerPos.X -= scrollSpeedMid;

            _farLayerPos.X -= scrollSpeedFar;
            _farMirrorLayerPos.X -= scrollSpeedFar;

            // Handle layer wrapping for close layer
            if (_originalPositionCloseLayer.X <= -_screenSize.X)
            {
                _originalPositionCloseLayer.X += 2 * _screenSize.X;
            }
            else if (_originalPositionCloseLayer.X > _screenSize.X)
            {
                _originalPositionCloseLayer.X -= 2 * _screenSize.X;
            }

            if (_originalPositionMirrorCloseLayer.X <= -_screenSize.X)
            {
                _originalPositionMirrorCloseLayer.X += 2 * _screenSize.X;
            }
            else if (_originalPositionMirrorCloseLayer.X > _screenSize.X)
            {
                _originalPositionMirrorCloseLayer.X -= 2 * _screenSize.X;
            }

            // Handle layer wrapping for mid layer
            if (_midLayerPos.X <= -_screenSize.X)
            {
                _midLayerPos.X += 2 * _screenSize.X;
            }
            else if (_midLayerPos.X > _screenSize.X)
            {
                _midLayerPos.X -= 2 * _screenSize.X;
            }

            if (_midMirrorLayerPos.X <= -_screenSize.X)
            {
                _midMirrorLayerPos.X += 2 * _screenSize.X;
            }
            else if (_midMirrorLayerPos.X > _screenSize.X)
            {
                _midMirrorLayerPos.X -= 2 * _screenSize.X;
            }

            // Handle layer wrapping for far layer
            if (_farLayerPos.X <= -_screenSize.X)
            {
                _farLayerPos.X += 2 * _screenSize.X;
            }
            else if (_farLayerPos.X > _screenSize.X)
            {
                _farLayerPos.X -= 2 * _screenSize.X;
            }

            if (_farMirrorLayerPos.X <= -_screenSize.X)
            {
                _farMirrorLayerPos.X += 2 * _screenSize.X;
            }
            else if (_farMirrorLayerPos.X > _screenSize.X)
            {
                _farMirrorLayerPos.X -= 2 * _screenSize.X;
            }

            // Update rectangle positions
            _closeLayerRect.X = (int)_originalPositionCloseLayer.X;
            _closeLayerMirrorRect.X = (int)_originalPositionMirrorCloseLayer.X;

            _midLayerRect.X = (int)_midLayerPos.X;
            _midMirrorLayerRect.X = (int)_midMirrorLayerPos.X;

            _farLayerRect.X = (int)_farLayerPos.X;
            _farMirrorLayerRect.X = (int)_farMirrorLayerPos.X;
        }
        public void BackgroundDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_staticBackground, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(_farLayer, _farLayerRect, Color.White);
            spriteBatch.Draw(_farLayer, _farMirrorLayerRect, Color.White);
            spriteBatch.Draw(_midLayer, _midLayerRect, Color.White);
            spriteBatch.Draw(_midLayer, _midMirrorLayerRect, Color.White);
            spriteBatch.Draw(_closeLayer, _closeLayerRect, Color.White);
            spriteBatch.Draw(_closeLayer, _closeLayerMirrorRect, Color.White);
            spriteBatch.End();
        }

    }
}
