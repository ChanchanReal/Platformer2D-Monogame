using Fallen_Knight.GameAssets.Animations;
using Microsoft.Xna.Framework;
using System;

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
