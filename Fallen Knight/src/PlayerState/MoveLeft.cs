using Fallen_Knight.GameAssets.Animations;
using Microsoft.Xna.Framework;
using System;

public class MoveLeft : InputCommand
{
    public MoveLeft(float maxWalkingspeed, float accel, float jumpSpeed, Vector2 playerSpeed, Animation animation) :
        base(maxWalkingspeed, accel, jumpSpeed, playerSpeed, animation)
    {
    }

    public override Vector2 Execute(GameTime gameTime)
    {
        // this decelerate the player faster if playerspeed is faster than zero bias to left.
        if (PlayerSpeed.X > 0)
        {
            PlayerSpeed.X -= Accel; 
        }
        PlayerSpeed.X = PlayerSpeed.X - Accel;
        PlayerSpeed.X = Math.Clamp(PlayerSpeed.X, -MaxWalkingSpeed, MaxWalkingSpeed);
        animationToPlay.UpdateFrame(gameTime);
        animationToPlay.FlipH = true;
        return PlayerSpeed;
    }
}
