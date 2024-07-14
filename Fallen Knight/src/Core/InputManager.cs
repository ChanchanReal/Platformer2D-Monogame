using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Fallen_Knight.src.Core
{
    public static class InputManager
    {
        private static KeyboardState _currentKeyboardState;
        private static KeyboardState _previousKeyboardState;
        private static MouseState _mouseState;
        private static MouseState _oldMouseState;
        private static Vector2 _mousePosition;

        public static bool Input(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }

        public static bool HoldableInput(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }

        public static void Update()
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();
            _oldMouseState = _mouseState;
            _mouseState = Mouse.GetState();
        }

        public static bool IsMouseLeftButtonDown()
        {
            return _mouseState.LeftButton == ButtonState.Pressed && _oldMouseState.LeftButton != ButtonState.Pressed;
        }

        public static void SetMousePosition(Vector2 newPosition)
        {
            _mousePosition = newPosition;
        }
        public static Vector2 GetMousePositionFromCamera()
        {
            return new Vector2(_mouseState.X, _mouseState.Y);
        }

        public static Vector2 GetMousePosition()
        {
            return _mousePosition;
        }
    }
}
