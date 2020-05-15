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

    internal class GhostCollision
    {
        public GhostCollision(Ghost ghost)
        {
            Ghost = ghost;
        }
        
        public Ghost Ghost { get; }
    }
}