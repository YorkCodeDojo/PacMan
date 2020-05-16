namespace NPacMan.Game.GhostStrategies
{
    public class StaysCloseToPacManLocation : IDirectToLocation
    {
        private readonly string _ghostName;
        public StaysCloseToPacManLocation(string ghostName)
        {
            _ghostName = ghostName;
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

            return new DirectToGhostScatterTarget(ghost);
        }
    }
}