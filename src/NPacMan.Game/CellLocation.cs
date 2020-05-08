using System;

namespace NPacMan.Game
{
    public readonly struct CellLocation
    {
        public int X { get; }
        public int Y { get; }

        public CellLocation(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static CellLocation TopLeft => new CellLocation(0, 0);

        public CellLocation Above => new CellLocation(X, Y - 1);

        public CellLocation Below => new CellLocation(X, Y + 1);

        public CellLocation Left => new CellLocation(X - 1, Y);

        public CellLocation Right => new CellLocation(X + 1, Y);

        public static implicit operator (int X, int Y)(CellLocation cellLocation)
                    => (cellLocation.X, cellLocation.Y);

        public static implicit operator CellLocation((int x, int y) location)
                    => new CellLocation(location.x, location.y);

        public static CellLocation operator +(CellLocation location, Direction direction)
            => direction switch
            {
                Direction.Up => location.Above,
                Direction.Down => location.Below,
                Direction.Left => location.Left,
                Direction.Right => location.Right,
                _ => location
            };

        public static int operator -(CellLocation from, CellLocation to) => (int)(Math.Sqrt(Math.Pow(to.X - from.X, 2) + Math.Pow(to.Y - from.Y, 2)));

    }
}
