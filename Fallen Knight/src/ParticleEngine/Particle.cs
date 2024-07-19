using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fallen_Knight.GameAssets
{
    public class Particle
    {
        public string ID = "Default";
        public Texture2D Texture;
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public float Angle;
        public float AngularVelocity;
        public float Size;
        public int TTL;
        private int InitialTTL;
        private float opacity = 1;

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
            InitialTTL = TTL;
        }

        public void Update()            
        {
            Position += Velocity;   
            Angle += AngularVelocity;
            TTL--;      

            // Smooth opacity transition
            opacity = MathHelper.Clamp((float)TTL / InitialTTL, 0, 1);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            spriteBatch.Draw(Texture, Position, sourceRectangle, Color * opacity,
                Angle, origin, Size, SpriteEffects.None, 0f);
        }
    }   
}
