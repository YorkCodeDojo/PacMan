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

    }
}
