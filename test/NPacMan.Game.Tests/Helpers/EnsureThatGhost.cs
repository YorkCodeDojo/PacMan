using System;

namespace NPacMan.Game.Tests.Helpers
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

        internal void IsEdible()
        {
            if (!_ghost.Edible)
                throw new Exception($"Ghost {_ghost.Name} should be edible");
        }

        internal void IsNotEdible()
        {
            if (_ghost.Edible)
                throw new Exception($"Ghost {_ghost.Name} should be not edible");
        }

    }
}
