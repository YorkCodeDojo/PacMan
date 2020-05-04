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
    public class DirectChaseToPacManStrategy : IGhostStrategy
    {
        public Direction? GetNextDirection(Ghost ghost, Game game)
        {
            var availableMoves = GetAvailableMovesForLocation(ghost.Location, game.Walls);
                
            availableMoves.Remove(ghost.Direction.Opposite());

            if (availableMoves.Count() == 1)
                return availableMoves.First();
            
            var pacMan = new CellLocation(game.PacMan.X, game.PacMan.Y);

            // return availableMoves
            //     .OrderBy(possibleDirection => CalculateDistance(ghost.Location + possibleDirection, pacMan)).First();
            
                        return availableMoves
                .OrderBy(possibleDirection => (ghost.Location + possibleDirection) - pacMan).First();

        }

    //    public int CalculateDistance(CellLocation from, CellLocation to) => Math.Abs(to.X - from.X) + Math.Abs(to.Y - from.Y);
        

        public List<Direction> GetAvailableMovesForLocation(CellLocation location, IReadOnlyCollection<(int x, int y)> walls)
        {
            var result = new List<Direction>(4);

            if (!walls.Contains(location.Left))
                result.Add(Direction.Left);

            if (!walls.Contains(location.Right))
                result.Add(Direction.Right);

            if (!walls.Contains(location.Above))
                result.Add(Direction.Up);

            if (!walls.Contains(location.Below))
                result.Add(Direction.Down);

            return result;                                             
        }

    }
}
