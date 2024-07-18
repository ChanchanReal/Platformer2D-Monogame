using Fallen_Knight.GameAssets.Animations;
using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.src.Core;
using Fallen_Knight.src.Interface;
using Fallen_Knight.src.PlayerState;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

public class MoveRight : PlayerState
{
    private float playerWalkingSpeed = 0.1f;
    bool execute = false;
    public MoveRight(Player player):base(player)
    {
    }

    public override void HandleInput(GameTime gameTime)
    {
        InvokeState(gameTime);
        HandleHorizontalInput();
        HandleCommonInput();
    }
    private void InvokeState(GameTime gameTime)
    {
            // this decelerate the player faster if playerspeed is faster than zero bias to right.
            float newPlayerSpeed = Player.PlayerSpeed.X + playerWalkingSpeed;
            if (newPlayerSpeed < 0) newPlayerSpeed += playerWalkingSpeed * 2;
            float clampX = Math.Clamp(newPlayerSpeed, -Player.MaxWalkingSpeed, Player.MaxWalkingSpeed);
            Player.PlayerSpeed = new Vector2(clampX, Player.PlayerSpeed.Y);
            Player.GenerateParticles(gameTime);
            execute = true;
        
    }
    private void HandleHorizontalInput()
    {
        if (InputManager.HoldableInput(Keys.Right) && Player.CollisionDirection != 1)
        {

        }
        else if (InputManager.HoldableInput(Keys.Left) && Player.CollisionDirection != -1)
        {
            if (!Player.IsDashing && !(Player.PlayerState is MoveLeft)) Player.SwitchState(new MoveLeft(Player));
        }
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
        Player.SpriteDirection = false;
    }
}
