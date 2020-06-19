using System.Linq;

namespace NPacMan.Game.GhostStrategies
{
    public class DirectToGhostHouseLocation : IDirectToLocation
    {
        public CellLocation GetLocation(Game game)
        {
            return game.GhostHouse.FirstOrDefault();
        }
    }
}