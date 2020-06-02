namespace PacManDebugger
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

        public override string ToString() => $"{X},{Y}";
    }
}
