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
    }
}
