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

    public class EnsureThatPacMan
    {
        private readonly PacMan _pacMan;

        public EnsureThatPacMan(PacMan pacMan)
        {
            _pacMan = pacMan;
        }

        internal void IsAt(CellLocation expectedLocation)
        {
            if (_pacMan.Location != expectedLocation)
                throw new Exception($"PacMan should be at {expectedLocation} not {_pacMan.Location}");
        }

    }
}
