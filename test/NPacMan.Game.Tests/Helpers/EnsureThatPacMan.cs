using System;

namespace NPacMan.Game.Tests.Helpers
{
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