using System;
using System.IO;
using System.Media;

namespace NPacMan.UI
{
    public class Sound
    {
        private readonly SoundPlayer _soundSource;
        public readonly TimeSpan RepeatTime;
        public readonly bool Interrupt;

        public Sound(UnmanagedMemoryStream file, int milliseconds, bool interrupt)
        {
            _soundSource = new SoundPlayer(file);
            _soundSource.Load();
            RepeatTime = TimeSpan.FromMilliseconds(milliseconds);
            Interrupt = interrupt;
        }

        public void Play()
        {
            _soundSource.Play();
        }
    }
}