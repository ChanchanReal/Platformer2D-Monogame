using Microsoft.VisualBasic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Fallen_Knight.src.Core
{
    public class GameSoundManager
    {
        SoundEffect backgroundSound;
        
        public GameSoundManager()
        {
        }
        public void LoadSounds(ContentManager content)
        {
            backgroundSound = content.Load<SoundEffect>("Sounds/Background/bg");
        }

        public void PlayWalkingSound()
        {

        }
        public void PlayBackgroundMusic()
        {
            float volume = 0.5f;
            float pitch = 0.5f;
            float pan = 0;
            SoundEffectInstance bg = backgroundSound.CreateInstance();
            bg.IsLooped = true;
            bg.Volume = volume;
            bg.Pitch = pitch;
            bg.Pan = pan;
            bg.Play();
        }
    }
}
