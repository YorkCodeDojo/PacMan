using NPacMan.Game;

namespace NPacMan.UI.Bots.Models
{
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