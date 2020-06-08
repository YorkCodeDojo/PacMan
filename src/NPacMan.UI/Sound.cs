using System;
using System.IO;
using System.Media;
using NPacMan.UI.Audio;

namespace NPacMan.UI
{
    internal class Sound
    {
        public readonly CachedSound SoundSource;
        public readonly TimeSpan RepeatTime;
        public readonly bool Interrupt;

        public Sound(UnmanagedMemoryStream file, int milliseconds, bool interrupt)
        {
            SoundSource = new CachedSound(file);
            RepeatTime = TimeSpan.FromMilliseconds(milliseconds);
            Interrupt = interrupt;
        }
    }
}