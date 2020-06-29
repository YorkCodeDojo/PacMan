namespace NPacMan.Game.GhostStrategies
{
    public class StaysCloseToPacManLocation : IDirectToLocation
    {
        private readonly string _ghostName;
        private readonly CellLocation _scatterTarget;

        public StaysCloseToPacManLocation(string ghostName, CellLocation scatterTarget)
        {
            _ghostName = ghostName;
            _scatterTarget = scatterTarget;
        }
        public CellLocation GetLocation(Game game)
        {
            var ghost = game.Ghosts[_ghostName];

            var directToLocation = GetDirectToLocation(game, ghost);

            return directToLocation.GetLocation(game);
        }

        public IDirectToLocation GetDirectToLocation(Game game, Ghost ghost)
        {
            var distance = ghost.Location - game.PacMan.Location;

            if(distance > 8) {
                return new DirectToPacManLocation();
            }

            return new DirectToGhostScatterTarget(_scatterTarget);
        }
    }
}