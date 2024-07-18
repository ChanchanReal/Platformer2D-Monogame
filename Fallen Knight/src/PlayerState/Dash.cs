using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.src.Core;
using Fallen_Knight.src.Interface;
using Fallen_Knight.src.PlayerState;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

public class Dash : PlayerState
{
    private float dashTime = 3;
    private float dashDuration = 0;
    private float dashSpeed = 15;
    public Dash(Player player) : base(player)
    {
    }

    public override void HandleInput(GameTime gameTime)
    {
        InvokeState(gameTime);
        HandleHorizontalInput();
        HandleCommonInput();
    }
    private void DoDash(GameTime gameTime)
    {
        if (!Player.IsDashing)
            return;

        if (Player.DashDuration > 0)
        {
            Player.DashDuration -= Player.DeltaTime;
            if (Player.SpriteDirection == false)
            {
                Player.PlayerSpeed = new Vector2(dashSpeed, 0);
            }
            else
            {
                Player.PlayerSpeed = new Vector2(-dashSpeed, 0);
            }

            Player.GenerateDashParticle(gameTime);
        }
        else
        {
            Player.DashDuration = 0;
            Player.IsDashing = false;

            if (Player.SpriteDirection)
            {
                Player.PlayerSpeed = new Vector2(-Player.MaxWalkingSpeed, Player.PlayerSpeed.Y);
                Player.SwitchState(new MoveLeft(Player));
            }
            else
            {
                Player.PlayerSpeed = new Vector2(Player.MaxWalkingSpeed, Player.PlayerSpeed.Y);
                Player.SwitchState(new MoveRight(Player));
            }
            // Reset speed after dash
        }
    }

    public void InvokeState(GameTime gameTime)
    {
        if (Player.IsDashing == false)
        {
            Player.IsDashing = true;
            Player.DashDuration = Player.DashTime;
            Player.dashCoolDown = 2f;
        }

        DoDash(gameTime);
        
    }

    private void HandleHorizontalInput()
    {
        if (InputManager.HoldableInput(Keys.Right) && Player.CollisionDirection != 1)
        {
            if (!Player.IsDashing) Player.SwitchState(new MoveRight(Player));
        }
        else if (InputManager.HoldableInput(Keys.Left) && Player.CollisionDirection != -1)
        {
            if (!Player.IsDashing) Player.SwitchState(new MoveLeft(Player));
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
        Player.CurrentAction = PlayerStatus.Dash;
    }
}
