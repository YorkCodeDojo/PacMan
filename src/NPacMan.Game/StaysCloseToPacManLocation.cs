namespace NPacMan.Game
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
            var pacManLocation = new CellLocation(game.PacMan.X, game.PacMan.Y);
            var distance = ghost.Location - pacManLocation;

            if(distance > 8) {
                return new DirectToPacManLocation();
            }

            return new DirectToGhostScatterTarget(ghost);
        }
    }
}