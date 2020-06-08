using System;
using System.Diagnostics;
using NPacMan.SharedUi.Properties;
using NPacMan.UI.Audio;

namespace NPacMan.UI
{
    public class SoundSet
    {
        private readonly Sound _eatFruit;
        private readonly Sound _chomp1;
        private readonly Sound _chomp2;
        private readonly Sound _beginning;
        private readonly Sound _death;
        private readonly Sound _intermission;
        private readonly Sound _extraPac;
        private readonly Sound _eatGhost;

        private DateTime _nextSound;

        private readonly AudioPlaybackEngine _audioPlaybackEngine;

        public SoundSet()
        {
            // Each sound has two parameters
            // Length (in milliseconds)
            // Whether the sound can interrupt the current sound

            _eatFruit = new Sound(Resources.pacman_eatfruit, 1000, true);
            _chomp1 = new Sound(Resources.pacman_chomp1, 120, false);
            _chomp2 = new Sound(Resources.pacman_chomp2, 120, false);
            _death = new Sound(Resources.pacman_death, 2000, true);
            _beginning = new Sound(Resources.pacman_beginning, 4000, true);
            _intermission = new Sound(Resources.pacman_intermission, 1000, true);
            _extraPac = new Sound(Resources.pacman_extrapac, 500, true);
            _eatGhost = new Sound(Resources.pacman_eatghost, 1000, true);

            _nextSound = DateTime.Now;

            _audioPlaybackEngine = new AudioPlaybackEngine(11025, 1);
        }

        private void Play(Sound sound)
        {
            if (DateTime.Now > _nextSound || sound.Interrupt)
            {
                _nextSound = DateTime.Now.Add(sound.RepeatTime);
                _audioPlaybackEngine.PlaySound(sound.SoundSource);
            }
        }

        private bool _chompSwitch;

        public void Chomp()
        {
            _chompSwitch = !_chompSwitch;
            Play(_chompSwitch ? _chomp1 : _chomp2);
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
