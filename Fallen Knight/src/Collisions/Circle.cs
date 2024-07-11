using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallen_Knight.GameAssets.Collisions
{
    public class Circle
    {
        public Vector2 Center;
        public float Radius;

        public Circle(Vector2 position, float radius)
        {
            Center = position;
            this.Radius = radius;
        }

        public bool Intersecting(Rectangle target)
        {
            float closestX = Math.Clamp(Center.X, target.Left, target.Right);
            float closestY = Math.Clamp(Center.Y, target.Top, target.Bottom);

            float distanceX = Center.X - closestX;
            float distanceY = Center.Y - closestY;

            float distanceSquared = distanceX * distanceX + distanceY * distanceY;

            return distanceSquared < Radius * Radius;
        }
    }
}
