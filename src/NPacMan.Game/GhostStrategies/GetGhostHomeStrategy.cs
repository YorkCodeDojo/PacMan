using System.Collections.Generic;
using System.Linq;

namespace NPacMan.Game.GhostStrategies
{
    public class GetGhostHomeStrategy : IGhostStrategy
    {
        public Direction? GetNextDirection(Ghost ghost, Game game)
        {
            var availableMoves = GetAvailableMovesForLocation(ghost.Location, game.Walls);

            availableMoves.Remove(ghost.Direction.Opposite());

            if (availableMoves.Count() == 1)
                return availableMoves.First();

            var target = HeadToDoor(ghost, game)
                ? game.Doors.First()
                : game.GhostHouseMiddle;    

            return availableMoves
                .OrderBy(possibleDirection => (ghost.Location + possibleDirection) - target)
                .ThenBy(p => (int)p)
                .First();

        }
        private bool HeadToDoor(Ghost ghost, Game game)
        {
            return !(game.GhostHouse.Contains(ghost.Location) || game.Doors.Contains(ghost.Location)) && game.Doors.Any();
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