using Fallen_Knight.GameAssets.Animations;
using Microsoft.Xna.Framework;
using System;

public class MoveRight : InputCommand
{
    public MoveRight(float maxWalkingspeed, float accel, float jumpSpeed, Vector2 playerSpeed, PlayerAnimation animation) :
        base(maxWalkingspeed, accel, jumpSpeed, playerSpeed, animation)
    {
    }

    public override Vector2 Execute(GameTime gameTime)
    {
        // this decelerate the player faster if playerspeed is faster than zero bias to right.
        if (PlayerSpeed.X < 0)
        {
            PlayerSpeed.X += Accel;
        }
        PlayerSpeed.X += Accel;
        PlayerSpeed.X = Math.Clamp(PlayerSpeed.X, -MaxWalkingSpeed, MaxWalkingSpeed);
        return PlayerSpeed;
    }
}
