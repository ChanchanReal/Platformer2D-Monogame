using Fallen_Knight.GameAssets.Animations;
using Microsoft.Xna.Framework;

public interface ICommand
{
    public Vector2 Execute(GameTime gameTime);
}

public abstract class InputCommand : ICommand
{
    protected float JumpSpeed;
    protected float MaxWalkingSpeed;
    protected float Accel;
    protected Vector2 PlayerSpeed;
    protected Animation animationToPlay;
    public InputCommand(float maxWalkingspeed, float accel,
        float jumpSpeed, Vector2 playerSpeed, Animation animationToPlay)
    {
        MaxWalkingSpeed = maxWalkingspeed;
        Accel = accel;
        JumpSpeed = jumpSpeed;
        PlayerSpeed = playerSpeed;
        this.animationToPlay = animationToPlay;
    }

    public abstract Vector2 Execute(GameTime gameTime);
}
