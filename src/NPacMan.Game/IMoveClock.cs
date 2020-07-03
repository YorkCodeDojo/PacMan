using System;

namespace NPacMan.Game
{
    public interface IMoveClock
    {
        bool ShouldGhostMove(int gameLevel, int coinsRemaining, string ghostName, GhostStatus status, bool inTunnel);
        bool ShouldPacManMove(int gameLevel, bool isFrightened);
        void UpdateTime(TimeSpan deltaTime);
    }
}