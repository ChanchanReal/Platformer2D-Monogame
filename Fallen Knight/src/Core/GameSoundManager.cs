using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Fallen_Knight.src.Core
{
    public class GameSoundManager
    {
        SoundEffect backgroundSound;
        SoundEffect fallingTileSound;
        private float fallingTileDuration;
        public GameSoundManager()
        {
        }
        public void LoadSounds(ContentManager content)
        {
            backgroundSound = content.Load<SoundEffect>("Sounds/Background/bg");
            fallingTileSound = content.Load<SoundEffect>("Sounds/Effect/earth rumble");
        }

        public void PlayWalkingSound()
        {
        }
        public SoundEffect GetFallingTileSound()
        {
            return fallingTileSound;

        }
        public void PlayBackgroundMusic()
        {
            float volume = 0.1f;
            SoundEffectInstance bg = backgroundSound.CreateInstance();
            bg.IsLooped = true;
            bg.Volume = volume;
            bg.Play();
        }
    }
}
