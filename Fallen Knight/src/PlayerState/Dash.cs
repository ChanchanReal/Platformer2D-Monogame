using Fallen_Knight.GameAssets.Animations;
using Microsoft.Xna.Framework;
using System;
using static Fallen_Knight.GameAssets.Character.Player;

public class Dash : InputCommand
{
    private float dashTime = 3;
    private float dashDuration = 0;
    private float dashSpeed = 15;
    public Dash(float maxWalkingspeed, float accel, float jumpSpeed, Vector2 playerSpeed, PlayerAnimation animation) :
         base(maxWalkingspeed, accel, jumpSpeed, playerSpeed, animation)
    {
    }

    public override Vector2 Execute(GameTime gameTime)
    {
        if (dashDuration <= 0)
        {
            dashDuration = dashTime;
        }

        if (dashDuration > 0)
        {
            dashDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            playerStatus = PlayerStatus.Dash;
            PlayerSpeed = new Vector2(dashSpeed, 0);
        }
        else
        {
            playerStatus = PlayerStatus.Idle;
        }

        return PlayerSpeed;
    }
}
