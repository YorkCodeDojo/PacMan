namespace NPacMan.Game
{
    public class InterceptPacManLocation : IDirectToLocation
    {
        private string _ghostName;
        public InterceptPacManLocation(string ghostName)
        {
            _ghostName = ghostName;
        }
        
        public CellLocation GetLocation(Game game)
        {            
            var ghost = game.Ghosts[_ghostName]
                                .Location;

            var pacman = new CellLocation(game.PacMan.X, game.PacMan.Y)
                + game.PacMan.Direction
                + game.PacMan.Direction;

            var xDiff = pacman.X - ghost.X;
            var yDiff = pacman.Y - ghost.Y;

            var xVector = xDiff * 2;
            var yVector = yDiff * 2;

            return new CellLocation(
                ghost.X + xVector,
                ghost.Y + yVector
            );
        }
    }
}