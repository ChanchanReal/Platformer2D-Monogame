using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallen_Knight.GameAssets.Bots
{
    [Obsolete("Todo adding more stuff here", false)]
    public class Bot
    {
        private float ExecutionDelay;
        private float Timer;
        private Random random;

        public Bot(float delay)
        {
            random = new Random();
            ExecutionDelay = delay;
            Timer = 0;
        }
        public void Move(ref bool movementDirection, float elapse)
        {
            if (Timer > 0)
            {
                Timer -= elapse;
            }
            else
            {
                int i = random.Next(0, 3);

                if (i == 1)
                {
                    movementDirection = false;
                }

                if (i == 2)
                {
                    movementDirection = true;
                }

                Timer = ExecutionDelay;
            }
        }
    }
}
