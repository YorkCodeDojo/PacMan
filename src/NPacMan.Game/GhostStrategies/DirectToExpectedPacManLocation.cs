namespace NPacMan.Game
{
    public class DirectToExpectedPacManLocation : IDirectToLocation
    {
        public CellLocation GetLocation(Game game)
        {
            return game.PacMan.Location
                + game.PacMan.Direction
                + game.PacMan.Direction
                + game.PacMan.Direction
                + game.PacMan.Direction;
        }
    }
}