using System;

namespace NPacMan.BotSDK.Models
{
    public struct CellLocation
    {
        public int X { get; set; }
        public int Y { get; set;  }

        public CellLocation(int x, int y)
        {
            X = x;
            Y = y;
        }
        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        public override string ToString() => $"{X},{Y}";

        public static CellLocation TopLeft => new CellLocation(0, 0);

        public CellLocation WithNewX(int newX) => new CellLocation(newX, Y);
        public CellLocation WithNewY(int newY) => new CellLocation(X, newY);

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

        public static int operator -(CellLocation from, CellLocation to)
            => (int)(Math.Sqrt(Math.Pow(to.X - from.X, 2) + Math.Pow(to.Y - from.Y, 2)));

        public override bool Equals(object? obj)
        {
            if (obj is CellLocation cellLocation)
            {
                return Equals(cellLocation);
            }
            return false;
        }

        public bool Equals(CellLocation cellLocation)
        {
            return (X == cellLocation.X) && (Y == cellLocation.Y);
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public static bool operator ==(CellLocation lhs, CellLocation rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(CellLocation lhs, CellLocation rhs)
        {
            return !lhs.Equals(rhs);
        }

    }
}
