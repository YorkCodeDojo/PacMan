namespace NPacMan.Game
{
    public class DirectToGhostScatterTarget : IDirectToLocation
    {
        private readonly CellLocation _home;
        public DirectToGhostScatterTarget(Ghost ghost)
        {
            _home = ghost.ScatterTarget;
        }

        public CellLocation GetLocation(Game game)
        {
            return _home;
        }
    }
}