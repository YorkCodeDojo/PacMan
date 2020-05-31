using System.Diagnostics.Tracing;

namespace NPacMan.Game
{
    class PacManEventSource : EventSource
    {
        public static PacManEventSource Log = new PacManEventSource();

        public void GhostMoved(string ghostName, int tickCounter, int fromLocationX, int fromLocationY, int toLocationX, int toLocationY)
        {
            WriteEvent(1, ghostName, tickCounter, fromLocationX, fromLocationY, toLocationX, toLocationY);
        }
    }
}
