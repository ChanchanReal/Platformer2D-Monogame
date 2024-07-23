using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallen_Knight.src.Items
{
    public abstract class Weapon
    {
        protected Texture2D texture;
        public Rectangle weaponRectangle;
        public Rectangle AttackHitBox;
        public Rectangle Hitbox;
        public float Damage = 10;
        public bool FlipH = false;

        public Weapon(Texture2D texture, Texture2D lighting, Rectangle rect)
        {
            this.texture = texture;
            weaponRectangle = rect;       
        }

        public abstract void Update(GameTime gameTime, Rectangle boundingRec);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    }

    public class Katana : Weapon
    {
        private float bounceTime = 0f;
        private float bounceSpeed = 2f; // Controls how fast the weapon bounces
        private float bounceHeight = 5f; // Controls the height of the bounce

        public Katana(Texture2D texture, Texture2D lighting, Rectangle rect) : base(texture, lighting, rect)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (FlipH)
            spriteBatch.Draw(texture, weaponRectangle,null ,Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f );
            else
            spriteBatch.Draw(texture, weaponRectangle,null ,Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f );
        }

        public override void Update(GameTime gameTime, Rectangle boundingRec)
        {
            PositionAndBounceWeapon(gameTime, boundingRec);
            AttackHitboxUpdate(boundingRec);
        }

        public void PositionAndBounceWeapon(GameTime gameTime, Rectangle boundingRec)
        {
            bounceTime += (float)gameTime.ElapsedGameTime.TotalSeconds * bounceSpeed;
            float bounceOffset = (float)Math.Sin(bounceTime) * bounceHeight;

            float yUpdate = (weaponRectangle.Height / 2) + 5;
            float newBounce = yUpdate + bounceOffset;
            weaponRectangle = new Rectangle(
            boundingRec.X,
            boundingRec.Y + (int)newBounce,
            weaponRectangle.Width,
            weaponRectangle.Height);
        }

        private void AttackHitboxUpdate(Rectangle boundingRec)
        {
            float scale = 1.5f;
            if (FlipH)
                AttackHitBox = new Rectangle(boundingRec.X - boundingRec.Width - 25, boundingRec.Y , (int)(64 * scale), (int)(34 * scale));
            else
                AttackHitBox = new Rectangle(boundingRec.X + boundingRec.Width, boundingRec.Y  , (int)(64 * scale), (int)(34 * scale));
        }
    }
}
