using System;
using System.Media;

namespace NPacMan.UI
{
    public class Sound
    {
        private readonly SoundPlayer _soundSource;
        public readonly TimeSpan RepeatTime;
        public readonly bool Interrupt;

        public Sound(string file, int milliseconds, bool interrupt)
        {
            _soundSource = new SoundPlayer($"sound\\pacman_{file}.wav");
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