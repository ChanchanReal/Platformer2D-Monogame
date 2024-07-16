using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.GameAssets.Interface;
using Fallen_Knight.GameAssets.Observer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Fallen_Knight.GameAssets.Camera
{
    public class Camera : IObserver
    {
        private readonly Random random;
        public Matrix transform;
        Viewport view;
        public Vector2 centre;
        float smoothness;
        float shakeIntensity;
        float shakeDuration;

        float zoom = 1.5f;

        public Camera(Viewport newView, float smoothness = 0.04f)
        {
            view = newView;
            this.smoothness = smoothness;
            random = new Random();
            ObserverManager.CameraEventSubscribe(this);
        }

        // Update method to follow the player
        public void Update(GameTime gameTime, Player player, Rectangle worldBounds)
        {
            // Calculate the target center of the camera based on the player's position
            Vector2 targetCentre = new Vector2(
                player.Position.X + player.BoundingRectangle.Width / 2,
                player.Position.Y + player.BoundingRectangle.Height / 2
            );

            // Smoothly follow the player
            SmoothFollow(ref centre, targetCentre, smoothness);

            // Clamp the camera position within the bounds of the game world

            Clamp(ref centre, worldBounds);

            if (shakeDuration >= 0)
            {
                centre += new Vector2(
                    (float)(random.NextDouble() * 2 - 1) * shakeIntensity,
                    (float)(random.NextDouble() * 2 - 1) * shakeIntensity
                    );

                shakeDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // Create the transform matrix
            transform = Matrix.CreateTranslation(new Vector3(-centre.X, -centre.Y, 0)) *
                        Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                        Matrix.CreateTranslation(new Vector3(view.Width / 2, view.Height / 2, 0));
        }

        private void Clamp(ref Vector2 position, Rectangle bounds)
        {
            position.X = MathHelper.Clamp(position.X, bounds.Left + view.Width / 2 / zoom, bounds.Right - view.Width / 2 / zoom);
            position.Y = MathHelper.Clamp(position.Y, bounds.Top + view.Height / 2 / zoom, bounds.Bottom - view.Height / 2 / zoom);
        }

        // Method to smoothly move the camera
        private void SmoothFollow(ref Vector2 position, Vector2 target, float smoothness)
        {
            position = Vector2.Lerp(position, target, smoothness);
        }

        private void CameraShake(float intensity, float duration)
        {
            shakeIntensity = intensity;
            shakeDuration = duration;
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            // Invert the camera transform matrix
            Matrix inverseTransform = Matrix.Invert(transform);

            // Transform the screen position to world coordinates
            return Vector2.Transform(screenPosition, inverseTransform);
        }

        public void Update()
        {
            float shake = 2f;
            float delay = 1.2f;
            CameraShake(shake, delay);
        }
    }
}
