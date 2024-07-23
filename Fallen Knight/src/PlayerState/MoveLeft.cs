using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.src.Core;
using Fallen_Knight.src.Interface;
using Fallen_Knight.src.PlayerState;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

public class MoveLeft : PlayerState
{
    float playerWalkSpeed = 0.1f;
    private bool execute = false;
    public MoveLeft(Player player) : base(player)
    {
    }

    public override void HandleInput(GameTime gameTime)
    {
        InvokeState(gameTime);
        HorizontalInput();
        HandleCommonInput();
    }

    private void InvokeState(GameTime gameTime)
    {
            // this decelerate the player faster if playerspeed is faster than zero bias to left.
            float newPlayerSpeed = Player.PlayerSpeed.X - playerWalkSpeed;
            if (newPlayerSpeed > 0) newPlayerSpeed -= playerWalkSpeed * 2;
            float clampX = Math.Clamp(newPlayerSpeed, -Player.MaxWalkingSpeed, Player.MaxWalkingSpeed);
            Player.PlayerSpeed = new Vector2(clampX, Player.PlayerSpeed.Y);
            Player.GenerateParticles(gameTime);
    }

    private void HorizontalInput()
    {
        // Handle input for moving right
        if (InputManager.HoldableInput(Keys.Right) && Player.CollisionDirection != 1)
        {
            if (!(Player.PlayerState is MoveRight) && !Player.IsDashing)
            {
                Player.SwitchState(new MoveRight(Player));
            }
        }
        // Continue moving left if input is held and no collision
        else if (InputManager.HoldableInput(Keys.Left) && Player.CollisionDirection != -1)
        {
            // Already in MoveLeft state, no need to switch
        }
        // Apply friction and switch to idle if necessary
        else
        {
            Player.ApplyFriction();

            if (Player.PlayerSpeed.X == 0 && Player.CollisionDirection == 0)
            {
                Player.SwitchState(new Idle(Player));
            }
        }
    }

    public override void UpdateState()
    {
        Player.CurrentAction = PlayerStatus.Walk;
        Player.SpriteDirection = FaceDirection.Left;
    }
}
