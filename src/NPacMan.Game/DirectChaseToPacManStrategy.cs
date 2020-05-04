using System;
using System.Collections.Generic;
using System.Linq;

namespace NPacMan.Game
{
    public static class DirectionExtentions
    {
        public static Direction Opposite(this Direction direction)
            => direction switch {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Right => Direction.Left,
                Direction.Left => Direction.Right,
                _ => throw new NotImplementedException()
            };
    }

    public interface IDirectToLocation
    {
        CellLocation GetLocation(Game game);
    }
    public class DirectToPacManLocation : IDirectToLocation
    {
        public CellLocation GetLocation(Game game)
        {
            return new CellLocation(game.PacMan.X, game.PacMan.Y);
        }
    }

    public class DirectToGhostHomeLocation : IDirectToLocation
    {
        private readonly CellLocation _home;
        public DirectToGhostHomeLocation(Ghost ghost)
        {
            _home = ghost.HomeLocation;
        }

        public CellLocation GetLocation(Game game)
        {
            return _home;
        }
    }

    public class DirectToStrategy : IGhostStrategy
    {
        private readonly IDirectToLocation _directToLocation;
        public DirectToStrategy(IDirectToLocation directToLocation)
        {
            _directToLocation = directToLocation;
        }
        public Direction? GetNextDirection(Ghost ghost, Game game)
        {
            var availableMoves = GetAvailableMovesForLocation(ghost.Location, game.Walls);
                
            availableMoves.Remove(ghost.Direction.Opposite());

            if (availableMoves.Count() == 1)
                return availableMoves.First();
            
            var target = _directToLocation.GetLocation(game);

            return availableMoves
                .OrderBy(possibleDirection => (ghost.Location + possibleDirection) - target).ThenBy(p => (int) p).First();

        }

        public List<Direction> GetAvailableMovesForLocation(CellLocation location, IReadOnlyCollection<(int x, int y)> walls)
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
