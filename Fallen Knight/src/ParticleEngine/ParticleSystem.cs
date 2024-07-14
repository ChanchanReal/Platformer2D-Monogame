using Fallen_Knight.GameAssets.Character;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Fallen_Knight.GameAssets
{
    public class ParticleSystem
    {
        public Random random;
        public Vector2 EmitterPosition;
        public List<Texture2D> Textures;
        public List<Particle> Particles;
        bool generate = false;
        public float DelayTime { get; set; } = 0.3f;

        public ParticleSystem(Vector2 emitterPosition, List<Texture2D> textures)
        {
            EmitterPosition = emitterPosition;
            Textures = textures;
            Particles = new List<Particle>();
            random = new Random();
        }
        private Particle GenerateParticles()
        {
            Texture2D texture = Textures[random.Next(Textures.Count)];
            Vector2 position = EmitterPosition;
            float angle = (float)(random.NextDouble() * Math.PI) + 3;
            float speed = (float)random.NextDouble() * 2;
            // Convert polar coordinates to Cartesian coordinates
            Vector2 velocity = new Vector2(
                (float)(speed * Math.Cos(angle)),
                (float)(speed * Math.Sin(angle))
            );
            Color color = Color.SaddleBrown;
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
            float size = 0.1f;
            int ttl = 5 + random.Next(10, 20);

            return new Particle(texture, position, velocity, color, angle, angularVelocity, size, ttl);
        }
        public void GenerateDashParticles()
        {
            Texture2D texture = Textures[1];
            Vector2 position = EmitterPosition + new Vector2(0, -50);
            Color color = Color.White;
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
            float size = 10f;
            int ttl = 5 + 60;

            for (int i = 0; i < random.Next(5, 10); i++)
                Particles.Add(new Particle(texture, position, 
                    new Vector2((float)random.NextDouble() * 2, (float)random.NextDouble() * 2)
                    , color, 0, angularVelocity, size, ttl));
        }

        public void Update(Player player)
        {
            int num = 10;
            
            if (StopGenerating())
            for (int i = 0; i < num; i++)
            {
                Particles.Add(GenerateParticles());
            }

            for (int particle = 0; particle < Particles.Count; particle++)
            {
                Particles[particle].Update();
                OnDestroy(particle);
            }

            UpdateEmitterLocation(player);

            generate = false;
        }

        private void UpdateEmitterLocation(Player player)
        {
            EmitterPosition = new Vector2(player.Position.X + (player.BoundingRectangle.Width / 2),
                player.Position.Y + (player.BoundingRectangle.Height / 2) + 40);
        }

        public bool StopGenerating()
        {
            return generate;
        }

        public void StartGeneratingParticle()
        {
            generate = true;
        }

        private void OnDestroy(int particle)
        {
            if (Particles[particle].TTL <= 0)
            {
                Particles.Remove(Particles[particle]);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            //sb.Begin();
            for (int i = 0; i < Particles.Count; i++)
            {
                Particles[i].Draw(sb);
            }
            //sb.End();
        }
    }
}
