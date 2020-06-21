using System.Collections.Generic;
using System.Linq;

namespace NPacMan.Game.GhostStrategies
{
    public class DirectToStrategy : IGhostStrategy
    {
        private readonly IDirectToLocation _directToLocation;
        private readonly bool _canWalkDoors;

        public DirectToStrategy(IDirectToLocation directToLocation, bool canWalkDoors = false)
        {
            _directToLocation = directToLocation;
            _canWalkDoors = canWalkDoors;
        }
        
        public Direction? GetNextDirection(Ghost ghost, Game game)
        {
            var placesCannotMove = game.WallsAndDoors.AsEnumerable();
            if(_canWalkDoors)
            {
                placesCannotMove = placesCannotMove.Except(game.Doors);
            }
            var availableMoves = GetAvailableMovesForLocation(ghost.Location, placesCannotMove);
                
            availableMoves.Remove(ghost.Direction.Opposite());

            if (availableMoves.Count() == 1)
                return availableMoves.First();
            
            var target = _directToLocation.GetLocation(game);

            return availableMoves
                .OrderBy(possibleDirection => (ghost.Location + possibleDirection) - target)
                .ThenBy(p => (int) p)
                .First();

        }

        private List<Direction> GetAvailableMovesForLocation(CellLocation location, IEnumerable<CellLocation> walls)
        {
            var result = new List<Direction>(4);

            if (!walls.Contains(location.Above))
                result.Add(Direction.Up);

            if (!walls.Contains(location.Below))
                result.Add(Direction.Down);

            if (!walls.Contains(location.Left))
                result.Add(Direction.Left);

            if (!walls.Contains(location.Right))
                result.Add(Direction.Right);

            return result;                                             
        }

    }
}