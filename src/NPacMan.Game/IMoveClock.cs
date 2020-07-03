using System;

namespace NPacMan.Game
{
    public interface IMoveClock
    {
        bool ShouldGhostMove(int gameLevel, string ghostName, GhostStatus status);
        bool ShouldPacManMove(int gameLevel, bool isFrightened);
        void UpdateTime(TimeSpan deltaTime);
    }
}