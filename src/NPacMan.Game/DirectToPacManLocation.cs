namespace NPacMan.Game
{
    public class DirectToPacManLocation : IDirectToLocation
    {
        public CellLocation GetLocation(Game game)
        {
            return new CellLocation(game.PacMan.X, game.PacMan.Y);
        }
    }
}