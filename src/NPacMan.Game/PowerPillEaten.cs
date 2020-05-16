namespace NPacMan.Game
{
    internal class PowerPillEaten
    {
        public PowerPillEaten(CellLocation location)
        {
            Location = location;
        }

        public CellLocation Location { get; }
    }
}