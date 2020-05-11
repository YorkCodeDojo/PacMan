using System;
using System.Collections.Generic;
using System.Text;

namespace NPacMan.Game
{
    /// <summary>
    /// Dummy class that just contains triggers to sounds
    /// </summary>
    public class NullSoundSet : ISoundSet
    {
        public void Chomp()
        {
        }

        public void EatFruit()
        {
        }

        public void Beginning()
        {
        }

        public void Death()
        {
        }

        public void Intermission()
        {
        }

        public void ExtraPac()
        {
        }

        public void EatGhost()
        {
        }
    }
}
