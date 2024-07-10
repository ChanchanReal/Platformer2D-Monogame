using Fallen_Knight.GameAssets.Animations;
using Microsoft.Xna.Framework;
using System;

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

public class MoveRight : InputCommand
{
    public MoveRight(float maxWalkingspeed, float accel, float jumpSpeed, Vector2 playerSpeed, Animation animation) :
        base(maxWalkingspeed, accel, jumpSpeed, playerSpeed, animation)
    {
    }

    public override Vector2 Execute(GameTime gameTime)
    {
        PlayerSpeed.X += Accel;
        PlayerSpeed.X = Math.Clamp(PlayerSpeed.X, -MaxWalkingSpeed, MaxWalkingSpeed);
        animationToPlay.UpdateFrame(gameTime);
        animationToPlay.FlipH = false;
        return PlayerSpeed;
    }
}

public class MoveLeft : InputCommand
{
    public MoveLeft(float maxWalkingspeed, float accel, float jumpSpeed, Vector2 playerSpeed, Animation animation) :
        base(maxWalkingspeed, accel, jumpSpeed, playerSpeed, animation)
    {
    }

    public override Vector2 Execute(GameTime gameTime)
    {
        PlayerSpeed.X = PlayerSpeed.X - Accel;
        PlayerSpeed.X = Math.Clamp(PlayerSpeed.X, -MaxWalkingSpeed, MaxWalkingSpeed);
        animationToPlay.UpdateFrame(gameTime);
        animationToPlay.FlipH = true;
        return PlayerSpeed;
    }
}

public class Dash : InputCommand
{
    public Dash(float maxWalkingspeed, float accel, float jumpSpeed, Vector2 playerSpeed, Animation animation) :
         base(maxWalkingspeed, accel, jumpSpeed, playerSpeed, animation)
    {
    }

    public override Vector2 Execute(GameTime gameTime)
    {
        PlayerSpeed.X += 10f;
        PlayerSpeed.X = Math.Clamp(PlayerSpeed.X, -MaxWalkingSpeed, 10);
        animationToPlay.UpdateFrame(gameTime);
        animationToPlay.FlipH = false;
        return PlayerSpeed;
    }
}

public class Jump : InputCommand
{
    public Jump(float maxWalkingspeed, float accel, float jumpSpeed, Vector2 playerSpeed, Animation animation) :
        base(maxWalkingspeed, accel, jumpSpeed, playerSpeed, animation)
    {
    }

    public override Vector2 Execute(GameTime gameTime)
    {
        PlayerSpeed.Y = JumpSpeed;
        animationToPlay.UpdateFrame(gameTime);
        return PlayerSpeed;
    }
}