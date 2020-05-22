using NPacMan.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace NPacMan.UI
{
    internal class GreedyBot : IBot
    {
        public GreedyBot()
        {
        }

        public Direction SuggestNextDirection(Game.Game game)
        {
            var currentLocation = game.PacMan.Location;

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
            shortestDistances[currentLocation.X, currentLocation.Y] = 0;
            visited[currentLocation.X, currentLocation.Y] = true;

            //3
            CellLocation? currentNode = currentLocation;

            while (currentNode is object)
            {
                foreach (var possibleDirection in GetAvailableMovesForLocation(currentNode.Value, game))
                {
                    var newLocation = currentNode.Value + possibleDirection;
                    if (newLocation.X < 0 || newLocation.X + 1 > game.Width) throw new Exception($"X is {newLocation.X} but the board width is only 0 to {game.Width}");
                    if (newLocation.Y < 0 || newLocation.Y + 1 > game.Height) throw new Exception($"Y is {newLocation.Y} but the board height is only 0 to {game.Height}");
                    if (!visited[newLocation.X, newLocation.Y])
                    {
                        var tentativeDistance = shortestDistances[currentNode.Value.X, currentNode.Value.Y] + 1;
                        var currentValue = shortestDistances[newLocation.X, newLocation.Y];
                        shortestDistances[newLocation.X, newLocation.Y] = Math.Min(tentativeDistance, currentValue);
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

            var nearestCoin = FindNearestCoin(game.Coins, shortestDistances);
            
            var bestDirectionToNearestCoin = DirectionToTarget(game, shortestDistances, nearestCoin);

            var nearestGhost = FindNearestGhost(game.Ghosts, shortestDistances);
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

        private Direction PickDifferentDirection(Game.Game game, CellLocation currentLocation, Direction directionToScaryGhost)
        {
            var possibleDirections = GetAvailableMovesForLocation(currentLocation, game);
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