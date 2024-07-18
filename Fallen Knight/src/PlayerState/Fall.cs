using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.src.Core;
using Fallen_Knight.src.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Fallen_Knight.src.PlayerState
{
    public class Fall : global::PlayerState
    {
        private float playerWalkingSpeed = 0.1f;
        public Fall(Player player) : base(player)
        {
        }

        public override void HandleInput(GameTime gameTime)
        {
            SwitchToIdle();
            HandleHorizontalInput(gameTime);
        }

        private void HandleHorizontalInput(GameTime gameTime)
        {
            if (InputManager.Input(Keys.D) && Player.dashCoolDown <= 0)
            {
                Player.SwitchState(new Dash(Player));
            }
            else if (InputManager.HoldableInput(Keys.Left))
            {
                float newPlayerSpeed = Player.PlayerSpeed.X - playerWalkingSpeed;
                if (newPlayerSpeed > 0) newPlayerSpeed -= playerWalkingSpeed * 2;
                float clampX = Math.Clamp(newPlayerSpeed, -Player.MaxWalkingSpeed, Player.MaxWalkingSpeed);
                Player.PlayerSpeed = new Vector2(clampX, Player.PlayerSpeed.Y);
                Player.GenerateParticles(gameTime);
                Player.SpriteDirection = true;
            }
            else if (InputManager.HoldableInput(Keys.Right))
            {
                float newPlayerSpeed = Player.PlayerSpeed.X + playerWalkingSpeed;
                if (newPlayerSpeed < 0) newPlayerSpeed += playerWalkingSpeed * 2;
                float clampX = Math.Clamp(newPlayerSpeed, -Player.MaxWalkingSpeed, Player.MaxWalkingSpeed);
                Player.PlayerSpeed = new Vector2(clampX, Player.PlayerSpeed.Y);
                Player.GenerateParticles(gameTime);
                Player.SpriteDirection = false;
            }
        }

        private void SwitchToIdle()
        {
            if (IsGrounded() && Math.Abs(Player.PlayerSpeed.Y) < 0.1f)
            {
                Player.SwitchState(new Idle(Player));
            }
        }
        private bool IsGrounded()
        {
            return Player.IsGround;
        }

        public override void UpdateState()
        {
            Player.CurrentAction = PlayerStatus.Fall;
        }
    }
}
