using NPacMan.Game;
using System;

namespace NPacMan.UI
{
    internal static class SoundSetExtensions
    {
        internal static Game.Game AddSounds(this Game.Game game)
        {
            var soundSet = new SoundSet();

            game.Subscribe(GameNotification.Beginning, soundSet.Beginning)
                .Subscribe(GameNotification.EatCoin, soundSet.Chomp)
                .Subscribe(GameNotification.Respawning, soundSet.Death)
                .Subscribe(GameNotification.EatFruit, soundSet.EatFruit)
                .Subscribe(GameNotification.EatGhost, soundSet.EatGhost)
                .Subscribe(GameNotification.ExtraPac, soundSet.ExtraPac)
                .Subscribe(GameNotification.Intermission, soundSet.Intermission);

            return game;
        }
    }

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

            _eatFruit = new Sound("eatfruit", 1000, true);
            _chomp = new Sound("chomp", 600, false);
            _death = new Sound("death", 2000, true);
            _beginning = new Sound("beginning", 4000, true);
            _intermission = new Sound("intermission", 1000, true);
            _extraPac = new Sound("extrapac", 500, true);
            _eatGhost = new Sound("eatghost", 1000, true);

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
