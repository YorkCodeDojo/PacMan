using System;

namespace NPacMan.Game.Tests
{
    public class EnsureThatGhost
    {
        private readonly Ghost _ghost;

        public EnsureThatGhost(Ghost ghost)
        {
            _ghost = ghost;
        }

        internal void IsAt(CellLocation expectedLocation)
        {
            if (_ghost.Location != expectedLocation)
                throw new Exception($"Ghost should be at {expectedLocation} not {_ghost.Location}");
        }

    }
}
