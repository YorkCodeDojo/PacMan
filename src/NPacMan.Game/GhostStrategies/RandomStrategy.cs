using System.Collections.Generic;
using System.Linq;

namespace NPacMan.Game.GhostStrategies
{
    public class RandomStrategy : IGhostStrategy
    {
        private readonly IDirectionPicker _directionPicker;
        public RandomStrategy(IDirectionPicker directionPicker)
        {
            _directionPicker = directionPicker;
        }
        public Direction? GetNextDirection(Ghost ghost, Game game)
        {
            var availableMoves = GetAvailableMovesForLocation(ghost.Location, game.Walls);
                
            availableMoves.Remove(ghost.Direction.Opposite());

            if (availableMoves.Count() == 1)
                return availableMoves.First();

            return _directionPicker.Pick(availableMoves);
        }
        
        private List<Direction> GetAvailableMovesForLocation(CellLocation location, IReadOnlyCollection<CellLocation> walls)
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