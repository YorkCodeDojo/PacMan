using System;

namespace NPacMan.Game.Tests
{
    public class TestMoveClock : IMoveClock
    {
        private int _counter=0;
        public bool ShouldGhostMove(Ghost ghost)
            => !ghost.Edible || _counter % 2 == 1;

        public bool ShouldPacManMove(int gameLevel, bool isFrightened)
            => true;

        public void UpdateTime(TimeSpan deltaTime) {
            _counter++;
        }
    }
}