using Fallen_Knight.GameAssets.Animations;
using Microsoft.Xna.Framework;

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