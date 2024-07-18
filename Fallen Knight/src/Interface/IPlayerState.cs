using Fallen_Knight.GameAssets.Animations;
using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.src.Core;
using Fallen_Knight.src.Interface;
using Fallen_Knight.src.PlayerState;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public interface IPlayerState
{
    public void UpdateState();
    public void HandleInput(GameTime gameTime);
}

public abstract class PlayerState : IPlayerState
{
    public Player Player;
    public PlayerState(Player player)
    {
        Player = player;
    }
    public abstract void UpdateState();
    public abstract void HandleInput(GameTime gameTime);

    /// <summary>
    /// This handle:
    /// Dash input,
    /// Gravity to fall,
    /// Notifying to jump when player close to ground
    /// Jump and Reset the Notifier 
    /// </summary>
    protected void HandleCommonInput()
    {
        DashInput();
        IfGravityPull();
        NotifyPlayerWantsToJump();
        Jump();
    }

    private void Jump()
    {
        if (Player.IsGround && !Player.HeadIsColliding && (Player.WantsToJump || InputManager.Input(Keys.Space)))
        {
            Player.IsGround = false;
            Player.IsJumping = true;
            Player.WantsToJump = false;
            Player.WantsToJumpDuration = 0f;
            Player.SwitchState(new Jump(Player));
        }
    }
    private void IfGravityPull()
    {
        if (Player.PlayerSpeed.Y > 0)
            Player.SwitchState(new Fall(Player));
    }

    private void NotifyPlayerWantsToJump()
    {
        if (!Player.IsGround && InputManager.Input(Keys.Space))
        {
            Player.WantsToJumpDuration = 0.2f;
            Player.WantsToJump = true;
        }
    }
    private void DashInput()
    {
        if (Player.dashCoolDown <= 0 && InputManager.Input(Keys.D)
            && (Player.CollisionDirection != -1 ||
            Player.CollisionDirection != 1))
        {
            Player.SwitchState(new Dash(Player));
        }
    }
}
