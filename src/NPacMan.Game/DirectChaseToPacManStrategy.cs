using System;
using System.Collections.Generic;
using System.Linq;

namespace NPacMan.Game
{
    public class DirectChaseToPacManStrategy : IGhostStrategy
    {
        public (int x, int y) Move(Ghost ghost, Game game)
        {
            var pacman = game.PacMan;
            var walls = game.Walls;

            var possibleGoodMoves = new List<(int x, int y)>();

            if (pacman.X < ghost.X)
            {
                if (TryMove(walls, ghost.X - 1, ghost.Y, out var newLocation))
                    possibleGoodMoves.Add(newLocation);
            }

            if (pacman.X > ghost.X)
            {
                if (TryMove(walls, ghost.X + 1, ghost.Y, out var newLocation))
                    possibleGoodMoves.Add(newLocation);
            }

            if (pacman.Y > ghost.Y)
            {
                if (TryMove(walls, ghost.X, ghost.Y + 1, out var newLocation))
                    possibleGoodMoves.Add(newLocation);
            }

            if (pacman.Y < ghost.Y)
            {
                if (TryMove(walls, ghost.X, ghost.Y - 1, out var newLocation))
                    possibleGoodMoves.Add(newLocation);
            }

            if (!possibleGoodMoves.Any())
            {
                if (TryMove(walls, ghost.X - 1, ghost.Y, out var newLocation1))
                    possibleGoodMoves.Add(newLocation1);

                if (TryMove(walls, ghost.X + 1, ghost.Y, out var newLocation2))
                    possibleGoodMoves.Add(newLocation2);

                if (TryMove(walls, ghost.X, ghost.Y + 1, out var newLocation3))
                    possibleGoodMoves.Add(newLocation3);

                if (TryMove(walls, ghost.X, ghost.Y - 1, out var newLocation4))
                    possibleGoodMoves.Add(newLocation4);
            }

            return possibleGoodMoves.First();

        }

        public bool TryMove(IReadOnlyCollection<(int x, int y)> walls, int newX, int newY, out (int x, int y) newLocation)
        {
            newLocation = (newX, newY);
            return !walls.Contains((newX, newY));
        }
    }
}
