using System;

namespace NPacMan.Game
{
    internal class Tick
    {
        public Tick(DateTime now)
        {
            Now = now;
        }
        public DateTime Now { get; }
    }
}