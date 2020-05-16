namespace NPacMan.Game
{
    internal class CoinCollision
    {
        public CoinCollision(CellLocation location)
        {
            Location = location;
        }

        public CellLocation Location { get; }
    }
}