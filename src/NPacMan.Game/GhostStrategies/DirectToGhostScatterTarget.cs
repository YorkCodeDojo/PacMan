namespace NPacMan.Game.GhostStrategies
{
    public class DirectToGhostScatterTarget : IDirectToLocation
    {
        public CellLocation ScatterTarget { get; }
        
        public DirectToGhostScatterTarget(CellLocation scatterTarget)
        {
            ScatterTarget = scatterTarget;
        }

        public CellLocation GetLocation(Game game)
        {
            return ScatterTarget;
        }
    }
}