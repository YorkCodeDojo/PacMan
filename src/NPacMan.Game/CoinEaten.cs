namespace NPacMan.Game
{
    internal class CoinEaten
    {
        public CoinEaten(CellLocation location)
        {
            Location = location;
        }

        public CellLocation Location { get; }
    }
}