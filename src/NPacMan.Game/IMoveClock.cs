using System;

namespace NPacMan.Game
{
    public interface IMoveClock
    {
        bool ShouldGhostMove(Ghost ghost);
        bool ShouldPacManMove(int gameLevel, bool isFrightened);
        void UpdateTime(TimeSpan deltaTime);
    }
}