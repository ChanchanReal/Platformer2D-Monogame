using Fallen_Knight.GameAssets.Character;
using Fallen_Knight.src.Core;
using Fallen_Knight.src.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallen_Knight.src.PlayerState
{
    public class Idle : global::PlayerState
    {

        public Idle(Player player) : base(player) { }
        public override void HandleInput(GameTime gameTime)
        {
            HorizontalMovement();
            HandleCommonInput();
        }

        private void HorizontalMovement()
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

                if (Player.PlayerSpeed.X == 0 && Player.CollisionDirection == 0 && Player.PlayerSpeed.Y == 0)
                {
                    Player.SwitchState(new Idle(Player));
                }
            }
        }

        public override void UpdateState()
        {
            Player.CurrentAction = PlayerStatus.Idle;
        }
    }
}
