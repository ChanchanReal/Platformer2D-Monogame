using Fallen_Knight.GameAssets.Animations;
using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.src.Core;
using Fallen_Knight.src.Interface;
using Fallen_Knight.src.PlayerState;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

public class Jump : PlayerState
{

    private float jumpSpeed = -8.5f;
    private bool execute = false;
    private float playerWalkingSpeed = 0.1f;
    public Jump(Player player) : base(player)
    {
    }

    public override void HandleInput(GameTime gameTime)
    {
        InvokeState(gameTime);
        HorizontalMovement(gameTime);
        HandleCommonInput();
        Player.SwitchState(new Fall(Player));
    }

    private void InvokeState(GameTime gameTime)
    {
        if (!execute)
        {
            Player.PlayerAnimation.Update(gameTime, Player.BoundingRectangle, Player.SpriteDirection, Player.CurrentAction);
            Player.PlayerSpeed = new Vector2(Player.PlayerSpeed.X, jumpSpeed);
            Player.IsGround = false;
            Player.IsJumping = true;
            Player.WantsToJump = false;
            Player.WantsToJumpDuration = 0f;
            execute = true;
        }
    }
    private void HorizontalMovement(GameTime gameTime)
    {
        if (InputManager.HoldableInput(Keys.Left))
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
    public override void UpdateState()
    {
        Player.CurrentAction = PlayerStatus.Jump;
    }
}