namespace PacManDebugger
{
    public readonly struct Location
    {
        public int X { get; }
        public int Y { get; }
        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"{X},{Y}";
    }
}
