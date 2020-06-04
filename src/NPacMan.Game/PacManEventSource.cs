using System;
using System.Diagnostics.Tracing;

namespace NPacMan.Game
{
    class PacManEventSource : EventSource
    {
        public static PacManEventSource Log = new PacManEventSource();

        internal void GameStarted(int width, int height, string wallsJson)
        {
            WriteEvent(1, width, height, wallsJson);
        }

        public void GhostMoved(string ghostName, int tickCounter, int fromLocationX, int fromLocationY, int toLocationX, int toLocationY)
        {
            WriteEvent(2, ghostName, tickCounter, fromLocationX, fromLocationY, toLocationX, toLocationY);
        }

        internal void GhostStuckInHouse(string ghostName, int tickCounter, int fromLocationX, int fromLocationY, int numberOfCoinsRequiredToExitHouse)
        {
            WriteEvent(3, ghostName, tickCounter, fromLocationX, fromLocationY, numberOfCoinsRequiredToExitHouse);
        }

        internal void GhostChangedState(string ghostName, int tickCounter, int locationX, int locationY, string strategy, bool edible)
        {
            WriteEvent(4, ghostName, tickCounter, locationX, locationY, strategy, edible);
        }

        internal void PacManMoved(int tickCounter, int fromLocationX, int fromLocationY, int toLocationX, int toLocationY)
        {
            WriteEvent(5, tickCounter, fromLocationX, fromLocationY, toLocationX, toLocationY);
        }

        internal void PacManStateChanged(int tickCounter, int lives, int score, string direction)
        {
            WriteEvent(6, tickCounter, lives, score, direction);
        }
    }
}
