using NPacMan.Game;

namespace NPacMan.UI.Bots
{
    public class BotPortal
    {
        public BotLocation Entry { get; set; }

        public BotLocation Exit { get; set; }
    }

    public readonly struct BotLocation
    {
        public int X { get; }

        public int Y { get; }

        public BotLocation(CellLocation location)
        {
            X = location.X;
            Y = location.Y;
        }
    }
}
