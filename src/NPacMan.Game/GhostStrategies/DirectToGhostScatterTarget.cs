namespace NPacMan.Game.GhostStrategies
{
    public class DirectToGhostScatterTarget : IDirectToLocation
    {
        private readonly CellLocation _home;
        public DirectToGhostScatterTarget(CellLocation scatterTarget)
        {
            _home = scatterTarget;
        }

        public CellLocation GetLocation(Game game)
        {
            return _home;
        }
    }
}