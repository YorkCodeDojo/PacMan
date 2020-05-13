namespace NPacMan.Game.GhostStrategies
{
    public class DirectToPacManLocation : IDirectToLocation
    {
        public CellLocation GetLocation(Game game)
        {
            return game.PacMan.Location;
        }
    }
}