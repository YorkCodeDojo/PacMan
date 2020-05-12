using System;

namespace NPacMan.Game.Tests.GameTests
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
            if (_ghost.Location.X != expectedLocation.X || _ghost.Location.Y != expectedLocation.Y)
                throw new Exception($"Ghost should be at {expectedLocation} not {_ghost.Location}");
        }

    }
}
