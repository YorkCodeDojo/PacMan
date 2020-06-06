namespace NPacMan.Game
{
    internal class FruitCollision
    {
        public FruitCollision(CellLocation location)
        {
            Location = location;
        }

        public CellLocation Location { get; }
    }
}