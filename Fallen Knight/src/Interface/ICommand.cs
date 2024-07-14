using Fallen_Knight.GameAssets.Animations;
using Microsoft.Xna.Framework;
using static Fallen_Knight.GameAssets.Character.Player;

public interface ICommand
{
    public Vector2 Execute(GameTime gameTime);
    public void GetPlayerState(PlayerStatus playerState);
    public PlayerStatus UpdatePlayerState(); 
}

public abstract class InputCommand : ICommand
{
    protected float JumpSpeed;
    protected float MaxWalkingSpeed;
    protected float Accel;
    protected Vector2 PlayerSpeed;
    protected PlayerAnimation animationToPlay;
    protected PlayerStatus playerStatus;
    public InputCommand(float maxWalkingspeed, float accel,
        float jumpSpeed, Vector2 playerSpeed, PlayerAnimation animationToPlay)
    {
        MaxWalkingSpeed = maxWalkingspeed;
        Accel = accel;
        JumpSpeed = jumpSpeed;
        PlayerSpeed = playerSpeed;
        this.animationToPlay = animationToPlay;
    }

    public abstract Vector2 Execute(GameTime gameTime);

    public void GetPlayerState(PlayerStatus playerState)
    {
        playerStatus = playerState;
    }

    public PlayerStatus UpdatePlayerState()
    {
        return playerStatus;
    }
}
