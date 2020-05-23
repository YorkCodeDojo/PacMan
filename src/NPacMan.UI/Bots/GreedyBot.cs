using NPacMan.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace NPacMan.UI
{

    internal class GreedyBot : IBot
    {
        private readonly LinkedCell[,] _cells;
        private readonly Game.Game _game;

        public GreedyBot(Game.Game game)
        {
            _cells = new LinkedCell[game.Width, game.Height];

            for (int row = 0; row < game.Height; row++)
            {
                for (int column = 0; column < game.Width; column++)
                {
                    if (!game.Walls.Contains((column, row)))
                    {
                        _cells[column, row] = new LinkedCell((column, row));
                    }
                }
            }

            for (int row = 0; row < game.Height; row++)
            {
                for (int column = 0; column < game.Width; column++)
                {
                    CellLocation location = (column, row);
                    if (!game.Walls.Contains((column, row)))
                    {
                        LinkedCell? above = null;
                        if (!game.Walls.Contains(location.Above) && location.Y > 0)
                        {
                            above = _cells[location.X, location.Y - 1];
                        }
                        else if (game.Portals.TryGetValue(location.Above, out var otherSideOfPortal))
                        {
                            var exit = otherSideOfPortal + Direction.Up;
                            above = _cells[exit.X, exit.Y];
                        }

                        LinkedCell? below = null;
                        if (!game.Walls.Contains(location.Below) && location.Y < game.Height - 1)
                        {
                            below = _cells[location.X, location.Y + 1];
                        }
                        else if (game.Portals.TryGetValue(location.Below, out var otherSideOfPortal))
                        {
                            var exit = otherSideOfPortal + Direction.Down;
                            below = _cells[exit.X, exit.Y];
                        }

                        LinkedCell? left = null;
                        if (!game.Walls.Contains(location.Left) && location.X > 0)
                        {
                            left = _cells[location.X - 1, location.Y];
                        }
                        else if (game.Portals.TryGetValue(location.Left, out var otherSideOfPortal))
                        {
                            var exit = otherSideOfPortal + Direction.Left;
                            left = _cells[exit.X, exit.Y];
                        }

                        LinkedCell? right = null;
                        if (!game.Walls.Contains(location.Right) && location.X < game.Width - 1)
                        {
                            right = _cells[location.X + 1, location.Y];
                        }
                        else if (game.Portals.TryGetValue(location.Right, out var otherSideOfPortal))
                        {
                            var exit = otherSideOfPortal + Direction.Right;
                            right = _cells[exit.X, exit.Y];
                        }


                        _cells[column, row].DefineNeighbours(left, right, above, below);
                    }
                }
            }

            this._game = game;
        }

        public Direction SuggestNextDirection()
        {
            var currentLocation = _cells[_game.PacMan.Location.X, _game.PacMan.Location.Y];

            var shortestDistances = CalculateDistances(_game, currentLocation);

            var nearestCoin = FindNearestCoin(_game.Coins, shortestDistances);

            Direction bestDirectionToNearestCoin;
            if (nearestCoin == CellLocation.TopLeft)
            {
                // No more coins are available,  game is meant to be finished.  Pick any direction
                // and hope to stay away from the ghosts
                bestDirectionToNearestCoin = currentLocation.AvailableMoves.First().Direction;
            }
            else
            {
                bestDirectionToNearestCoin = DirectionToTarget(shortestDistances, nearestCoin);
            }

            var nearestGhost = FindNearestGhost(_game.Ghosts, shortestDistances);

            bestDirectionToNearestCoin = AvoidGhost(_game, currentLocation, shortestDistances, bestDirectionToNearestCoin, nearestGhost);

            return bestDirectionToNearestCoin;
        }


        private Direction AvoidGhost(Game.Game game,
                                     LinkedCell currentLocation,
                                     int[,] shortestDistances, Direction bestDirectionToNearestCoin, Ghost? nearestGhost)
        {
            if (nearestGhost is object)
            {
                var distanceToGhost = shortestDistances[nearestGhost.Location.X, nearestGhost.Location.Y];
                if (distanceToGhost < 5)
                {
                    var directionToScaryGhost = DirectionToTarget(shortestDistances, nearestGhost.Location);
                    if (directionToScaryGhost == bestDirectionToNearestCoin)
                    {
                        bestDirectionToNearestCoin = PickDifferentDirection(currentLocation, directionToScaryGhost);
                    }
                }
            }

            return bestDirectionToNearestCoin;
        }

        private int[,] CalculateDistances(Game.Game game, LinkedCell currentLocation)
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
                var linkedCell = _cells[currentNode.Value.X, currentNode.Value.Y];

                foreach (var (_, newLocation) in linkedCell.AvailableMoves)
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

        private Direction PickDifferentDirection(LinkedCell currentLocation, Direction directionToScaryGhost)
        {
            var possibleDirections = currentLocation.AvailableMoves.ToList();
            var directionWeDontWantToTake = possibleDirections.Single(d => d.Direction == directionToScaryGhost);
            possibleDirections.Remove(directionWeDontWantToTake);

            return possibleDirections.First().Direction;
        }

        private Direction DirectionToTarget(int[,] shortestDistances, CellLocation targetLocation)
        {
            var bestDirection = default(Direction);
            var bestNewNode = default(LinkedCell);
            var distanceToCoin = shortestDistances[targetLocation.X, targetLocation.Y];
            var currentNode = _cells[targetLocation.X, targetLocation.Y];
            while (distanceToCoin > 0)
            {
                var bestScore = int.MaxValue;
                foreach (var (possibleDirection, possibleLocation) in currentNode!.AvailableMoves)
                {
                    if (shortestDistances[possibleLocation!.Location.X, possibleLocation.Location.Y] < bestScore)
                    {
                        bestScore = shortestDistances[possibleLocation.Location.X, possibleLocation.Location.Y];
                        bestDirection = possibleDirection;
                        bestNewNode = possibleLocation;
                    }
                }

                currentNode = bestNewNode;
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
    }
}