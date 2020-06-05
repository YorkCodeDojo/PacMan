using System;
using NPacMan.SharedUi.Properties;

namespace NPacMan.UI
{
    public class SoundSet
    {
        private readonly Sound _eatFruit;
        private readonly Sound _chomp;
        private readonly Sound _beginning;
        private readonly Sound _death;
        private readonly Sound _intermission;
        private readonly Sound _extraPac;
        private readonly Sound _eatGhost;

        private DateTime _nextSound;

        public SoundSet()
        {
            // Each sound has two parameters
            // Length (in milliseconds)
            // Whether the sound can interrupt the current sound

            _eatFruit = new Sound(Resources.pacman_eatfruit, 1000, true);
            _chomp = new Sound(Resources.pacman_chomp, 600, false);
            _death = new Sound(Resources.pacman_death, 2000, true);
            _beginning = new Sound(Resources.pacman_beginning, 4000, true);
            _intermission = new Sound(Resources.pacman_intermission, 1000, true);
            _extraPac = new Sound(Resources.pacman_extrapac, 500, true);
            _eatGhost = new Sound(Resources.pacman_eatghost, 1000, true);

            _nextSound = DateTime.Now;
        }

        private void Play(Sound sound)
        {
            if (DateTime.Now > _nextSound || sound.Interrupt)
            {
                sound.Play();
                _nextSound = DateTime.Now.Add(sound.RepeatTime);
            }
        }

        public void Chomp()
        {
            Play(_chomp);
        }

        public void EatFruit()
        {
            Play(_eatFruit);
        }

        public void Beginning()
        {
            Play(_beginning);
        }

        public void Death()
        {
            Play(_death);
        }

        public void Intermission()
        {
            Play(_intermission);
        }

        public void ExtraPac()
        {
            Play(_extraPac);
        }

        public void EatGhost()
        {
            Play(_eatGhost);
        }
    }
}
