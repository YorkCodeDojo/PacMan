namespace NPacMan.Game
{
    public class Fruit
    {
        public CellLocation Location { get; }
        public FruitType Type { get; }

        public Fruit(CellLocation location, FruitType type)
        {
            this.Location = location;
            this.Type = type;
        }
    }
}