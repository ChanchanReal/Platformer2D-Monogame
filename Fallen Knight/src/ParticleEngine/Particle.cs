using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fallen_Knight.GameAssets
{
    public class Particle
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public float Angle;
        public float AngularVelocity;
        public float Size;
        public int TTL;
        private int CloseEndLife;

        public Particle(Texture2D texture, Vector2 position, Vector2 velocity, 
            Color color, float angle, float angularVelocity, float size, int tTL)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Color = color;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Size = size;
            TTL = tTL;
            CloseEndLife = (int)(TTL * 0.8f);
        }

        public void Update()
        {
            Position = Position + Velocity;
            Angle += AngularVelocity;
            TTL--;

            if (TTL == CloseEndLife)
            {
                Velocity += new Vector2(0, 0.5f);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(0,0, Texture.Width, Texture.Height);
            Vector2 origin = new Vector2((Texture.Width / 2), (Texture.Height / 2));
            spriteBatch.Draw(Texture, Position, sourceRectangle, Color, 
                Angle, origin, Size, SpriteEffects.None, 0f);
        }
    }
}
