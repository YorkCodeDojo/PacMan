using System;

namespace NPacMan.Game.Tests.Helpers
{
    public class EnsureThatPacMan
    {
        private readonly Game _game;
        private readonly PacMan _pacMan;

        public EnsureThatPacMan(Game game, PacMan pacMan)
        {
            _game = game;
            _pacMan = pacMan;
        }

        internal void IsAt(CellLocation expectedLocation)
        {
            if (_pacMan.Location != expectedLocation)
                throw new Exception($"PacMan should be at {expectedLocation} not {_pacMan.Location}");
        }

        internal void HasLives(int expectedLives)
        {
            if (_game.Lives != expectedLives)
            {
                throw new Exception($"Lives should be {expectedLives} not {_game.Lives}.");
            }
        }
    }
}