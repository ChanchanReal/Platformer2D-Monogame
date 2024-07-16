using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallen_Knight.src.Interface
{
    public enum FaceDirection
    {
        Left = 0,
        Right = 1
    }

    public enum PlayerStatus
    {
        Idle = 0,
        Walk = 1,
        Jump = 2,
        Fall = 3,
        Attack = 4,
        Dash = 5,
    }
}
