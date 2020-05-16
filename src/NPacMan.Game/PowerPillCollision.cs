namespace NPacMan.Game
{
    internal class PowerPillCollision
    {
        public PowerPillCollision(CellLocation location)
        {
            Location = location;
        }

        public CellLocation Location { get; }
    }
}