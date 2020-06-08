using NPacMan.SharedUi.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace NPacMan.Blazor
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

        private readonly IJSRuntime _jsRuntime;

        public SoundSet(IJSRuntime jsRuntime)
        {
            _eatFruit = new Sound(jsRuntime, Resources.pacman_eatfruit, 0, 1000, true);
            _chomp1 = new Sound(jsRuntime, Resources.pacman_chomp1, 1, 120, false);
            _chomp2 = new Sound(jsRuntime, Resources.pacman_chomp2, 2, 120, false);
            _death = new Sound(jsRuntime, Resources.pacman_death, 3, 2000, true);
            _beginning = new Sound(jsRuntime, Resources.pacman_beginning, 4, 4000, true);
            _intermission = new Sound(jsRuntime, Resources.pacman_intermission, 5, 1000, true);
            _extraPac = new Sound(jsRuntime, Resources.pacman_extrapac, 6, 500, true);
            _eatGhost = new Sound(jsRuntime, Resources.pacman_eatghost, 7, 1000, true);

            _jsRuntime = jsRuntime;
        }

        private void Play(Sound sound)
        {
            if (DateTime.Now > _nextSound || sound.CanInterrupt)
            {
                _jsRuntime.InvokeVoidAsync("PlaySound", sound.Number);
                _nextSound = DateTime.Now.Add(sound.RepeatTime);
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
