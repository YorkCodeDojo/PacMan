using System;
using System.Collections.Generic;

namespace NPacMan.Game.Tests.Helpers
{
    public class EnsureThatGhosts
    {
        private readonly IEnumerable<Ghost> _ghosts;

        public EnsureThatGhosts(IEnumerable<Ghost> ghosts)
        {
            _ghosts = ghosts;
        }

        internal void IsAt(CellLocation expectedLocation)
        {
            foreach (var ghost in _ghosts)
            {
                if (ghost.Location != expectedLocation)
                    throw new Exception($"Ghost should be at {expectedLocation} not {ghost.Location}");
            }
        }

    }
}