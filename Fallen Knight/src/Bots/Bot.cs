using Fallen_Knight.src.Interface;
using System;

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
        public void Move(ref FaceDirection movementDirection, float elapse)
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
                    movementDirection = FaceDirection.Right;
                }

                if (i == 2)
                {
                    movementDirection = FaceDirection.Left;
                }

                Timer = ExecutionDelay;
            }
        }
    }
}
