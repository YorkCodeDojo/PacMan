namespace NPacMan.Game
{
    public class DirectToPacManLocation : IDirectToLocation
    {
        public CellLocation GetLocation(Game game)
        {
            return game.PacMan.Location;
        }
    }
}