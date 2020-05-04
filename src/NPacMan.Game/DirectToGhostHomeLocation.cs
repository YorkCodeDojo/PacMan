namespace NPacMan.Game
{
    public class DirectToGhostHomeLocation : IDirectToLocation
    {
        private readonly CellLocation _home;
        public DirectToGhostHomeLocation(Ghost ghost)
        {
            _home = ghost.HomeLocation;
        }

        public CellLocation GetLocation(Game game)
        {
            return _home;
        }
    }
}