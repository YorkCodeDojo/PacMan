using NPacMan.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NPacMan.UI
{
    public class LinkedCell
    {
        public LinkedCell(CellLocation location, LinkedCell? left, LinkedCell? right, LinkedCell? up, LinkedCell? down)
        {
            Location = location;
            Left = left;
            Right = right;
            Up = up;
            Down = down;

            var tempAvailableMoves = new List<LinkedCell>(4);
            var tempAvailableDirections = new List<Direction>(4);
            
            if (left is object)
            {
                tempAvailableMoves.Add(left);
                tempAvailableDirections.Add(Direction.Left);
            }

            if (right is object)
            {
                tempAvailableMoves.Add(right);
                tempAvailableDirections.Add(Direction.Right);
            }

            if (up is object)
            {
                tempAvailableMoves.Add(up);
                tempAvailableDirections.Add(Direction.Up);
            }

            if (down is object)
            {
                tempAvailableMoves.Add(down);
                tempAvailableDirections.Add(Direction.Down);
            }

            AvailableMoves = tempAvailableMoves.ToArray();
            AvailableDirections = tempAvailableDirections.ToArray();
        }

        public CellLocation Location { get; }
        public LinkedCell? Left { get; }
        public LinkedCell? Right { get; }
        public LinkedCell? Up { get; }
        public LinkedCell? Down { get; }
        public LinkedCell[] AvailableMoves { get; }
        public Direction[] AvailableDirections { get; }
    }

    internal class GreedyBot : IBot
    {
        public GreedyBot()
        {
        }

        private LinkedCell CreateCell(Lazy<LinkedCell>[,] cells, Game.Game game, CellLocation location)
        {

            LinkedCell? above = null;
            if (!game.Walls.Contains(location.Above) && location.Y > 0)
                above = cells[location.X, location.Y - 1].Value;

            LinkedCell? below = null;
            if (!game.Walls.Contains(location.Below) && location.Y < game.Height - 1)
                below = cells[location.X, location.Y + 1].Value;

            LinkedCell? left = null;
            if (!game.Walls.Contains(location.Left) && location.X > 0)
                left = cells[location.X - 1, location.Y].Value;

            LinkedCell? right = null;
            if (!game.Walls.Contains(location.Right) && location.X < game.Width - 1)
                right = cells[location.X + 1, location.Y].Value;

            return new LinkedCell(location, left, right, above, below);
        }


        public Direction SuggestNextDirection(Game.Game game)
        {
            var cells = new Lazy<LinkedCell>[game.Width, game.Height];

            for (int row = 0; row < game.Height; row++)
            {
                for (int column = 0; column < game.Width; column++)
                {
                    if (!game.Walls.Contains((column, row)))
                    {
                        var c = column;
                        var r = row;

                        cells[column, row] = new Lazy<LinkedCell>(() => {
                            return CreateCell(cells, game, (c, r));
                        });
                    }
                }
            }


            var currentLocation = cells[game.PacMan.Location.X, game.PacMan.Location.Y].Value;

            var shortestDistances = CalculateDistances(game, currentLocation, cells);

            var nearestCoin = FindNearestCoin(game.Coins, shortestDistances);

            var bestDirectionToNearestCoin = DirectionToTarget(game, shortestDistances, nearestCoin);

            var nearestGhost = FindNearestGhost(game.Ghosts, shortestDistances);

            bestDirectionToNearestCoin = AvoidGhost(game, currentLocation, shortestDistances, bestDirectionToNearestCoin, nearestGhost);

            return bestDirectionToNearestCoin;
        }


        private Direction AvoidGhost(Game.Game game, LinkedCell currentLocation, int[,] shortestDistances, Direction bestDirectionToNearestCoin, Ghost? nearestGhost)
        {
            if (nearestGhost is object)
            {
                var distanceToGhost = shortestDistances[nearestGhost.Location.X, nearestGhost.Location.Y];
                if (distanceToGhost < 5)
                {
                    var directionToScaryGhost = DirectionToTarget(game, shortestDistances, nearestGhost.Location);
                    if (directionToScaryGhost == bestDirectionToNearestCoin)
                    {
                        bestDirectionToNearestCoin = PickDifferentDirection(game, currentLocation, directionToScaryGhost);
                    }
                }
            }

            return bestDirectionToNearestCoin;
        }

        private int[,] CalculateDistances(Game.Game game, LinkedCell currentLocation, Lazy<LinkedCell>[,] cells)
        {
            //1
            var shortestDistances = new int[game.Width, game.Height];
            var visited = new bool[game.Width, game.Height];
            for (int r = 0; r < game.Height; r++)
            {
                for (int c = 0; c < game.Width; c++)
                {
                    shortestDistances[c, r] = int.MaxValue;
                }
            }

            //2
            shortestDistances[currentLocation.Location.X, currentLocation.Location.Y] = 0;
            visited[currentLocation.Location.X, currentLocation.Location.Y] = true;

            //3
            CellLocation? currentNode = currentLocation.Location;

            while (currentNode is object)
            {
                var distanceToCurrentNode = shortestDistances[currentNode.Value.X, currentNode.Value.Y];
                var linkedCell = cells[currentNode.Value.X, currentNode.Value.Y].Value;

                foreach (var newLocation in linkedCell.AvailableMoves)
                {
                    if (!visited[newLocation.Location.X, newLocation.Location.Y])
                    {
                        var tentativeDistance = distanceToCurrentNode + 1;
                        var currentValue = shortestDistances[newLocation.Location.X, newLocation.Location.Y];
                        shortestDistances[newLocation.Location.X, newLocation.Location.Y] = Math.Min(tentativeDistance, currentValue);
                    }
                }

                //4
                visited[currentNode.Value.X, currentNode.Value.Y] = true;


                var bestScore = int.MaxValue;
                currentNode = null;
                for (int r = 0; r < game.Height; r++)
                {
                    for (int c = 0; c < game.Width; c++)
                    {
                        var seen = visited[c, r];
                        if (!seen)
                        {
                            if (shortestDistances[c, r] < bestScore)
                            {
                                bestScore = shortestDistances[c, r];
                                currentNode = new CellLocation(c, r);
                            }
                        }
                    }
                }

            }

            return shortestDistances;
        }

        private Direction PickDifferentDirection(Game.Game game, LinkedCell currentLocation, Direction directionToScaryGhost)
        {
            var possibleDirections = currentLocation.AvailableDirections.ToList();
            possibleDirections.Remove(directionToScaryGhost);

            return possibleDirections.First();
        }

        private Direction DirectionToTarget(Game.Game game, int[,] shortestDistances, CellLocation targetLocation)
        {
            var bestDirection = default(Direction);
            var distanceToCoin = shortestDistances[targetLocation.X, targetLocation.Y];
            var currentNode = targetLocation;
            while (distanceToCoin > 0)
            {
                var bestScore = int.MaxValue;

                foreach (var possibleDirection in GetAvailableMovesForLocation(currentNode, game))
                {
                    var possibleLocation = currentNode + possibleDirection;
                    if (shortestDistances[possibleLocation.X, possibleLocation.Y] < bestScore)
                    {
                        bestScore = shortestDistances[possibleLocation.X, possibleLocation.Y];
                        bestDirection = possibleDirection;
                    }
                }

                currentNode = currentNode + bestDirection;
                distanceToCoin--;
            }

            return bestDirection.Opposite();
        }

        private CellLocation FindNearestCoin(IReadOnlyCollection<CellLocation> coins, int[,] shortestDistances)
        {
            var bestCoin = default(CellLocation);
            var shortestDistance = int.MaxValue;

            foreach (var coin in coins)
            {
                var distanceToCoin = shortestDistances[coin.X, coin.Y];

                if (distanceToCoin < shortestDistance)
                {
                    shortestDistance = distanceToCoin;
                    bestCoin = coin;
                }
            }

            return bestCoin;
        }

        private Ghost? FindNearestGhost(IReadOnlyDictionary<string, Ghost> ghosts, int[,] shortestDistances)
        {
            var closestGhost = default(Ghost);
            var shortestDistance = int.MaxValue;

            foreach (var ghost in ghosts.Values.Where(g => !g.Edible))
            {
                var distanceToGhost = shortestDistances[ghost.Location.X, ghost.Location.Y];

                if (distanceToGhost < shortestDistance)
                {
                    shortestDistance = distanceToGhost;
                    closestGhost = ghost;
                }
            }

            return closestGhost;
        }

        private List<Direction> GetAvailableMovesForLocation(CellLocation location, Game.Game game)
        {
            //Currently ignores portals

            var result = new List<Direction>(4);

            if (!game.Walls.Contains(location.Above) && location.Y > 0)
                result.Add(Direction.Up);

            if (!game.Walls.Contains(location.Below) && location.Y < game.Height - 1)
                result.Add(Direction.Down);

            if (!game.Walls.Contains(location.Left) && location.X > 0)
                result.Add(Direction.Left);

            if (!game.Walls.Contains(location.Right) && location.X < game.Width - 1)
                result.Add(Direction.Right);

            return result;
        }
    }
}