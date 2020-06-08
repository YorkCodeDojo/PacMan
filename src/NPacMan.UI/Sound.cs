using System;
using System.IO;
using System.Media;
using NPacMan.UI.Audio;

namespace NPacMan.UI
{
    public class Sound
    {
        private readonly CachedSound _soundSource;
        public readonly TimeSpan RepeatTime;
        public readonly bool Interrupt;

        public Sound(UnmanagedMemoryStream file, int milliseconds, bool interrupt)
        {
            _soundSource = new CachedSound(file);
            RepeatTime = TimeSpan.FromMilliseconds(milliseconds);
            Interrupt = interrupt;
        }

        public void Play()
        {
            AudioPlaybackEngine.Instance.PlaySound(_soundSource);
        }
    }
}