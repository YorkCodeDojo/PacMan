namespace NPacMan.Game
{
    public class DirectToExpectedPacManLocation : IDirectToLocation
    {
        public CellLocation GetLocation(Game game)
        {
            return new CellLocation(game.PacMan.X, game.PacMan.Y)
                + game.PacMan.Direction
                + game.PacMan.Direction
                + game.PacMan.Direction
                + game.PacMan.Direction;
        }
    }
}