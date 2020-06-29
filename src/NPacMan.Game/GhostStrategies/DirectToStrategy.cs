using System.Collections.Generic;
using System.Linq;

namespace NPacMan.Game.GhostStrategies
{
    public class DirectToStrategy : IGhostStrategy
    {
        public IDirectToLocation DirectToLocation { get; }
        private readonly bool _allowGhostsToWalkThroughDoors;

        public DirectToStrategy(IDirectToLocation directToLocation, bool allowGhostsToWalkThroughDoors = false)
        {
            DirectToLocation = directToLocation;
            _allowGhostsToWalkThroughDoors = allowGhostsToWalkThroughDoors;
        }

        public Direction? GetNextDirection(Ghost ghost, Game game)
        {
            var placesGhostsCannotMove = _allowGhostsToWalkThroughDoors
                                          ? game.Walls : game.WallsAndDoors;

            var availableMoves = GetAvailableMovesForLocation(ghost.Location, placesGhostsCannotMove);

            availableMoves.Remove(ghost.Direction.Opposite());

            if (availableMoves.Count() == 1)
                return availableMoves.First();

            var target = DirectToLocation.GetLocation(game);

            return availableMoves
                .OrderBy(possibleDirection => (ghost.Location + possibleDirection) - target)
                .ThenBy(p => (int)p)
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