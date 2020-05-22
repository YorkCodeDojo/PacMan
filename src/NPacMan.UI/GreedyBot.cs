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

            var bestDirection = default(Direction);
            var distanceToCoin = shortestDistances[nearestCoin.X, nearestCoin.Y];
            currentNode = nearestCoin;
            while (distanceToCoin > 0)
            {
                var bestScore = int.MaxValue;
                
                foreach (var possibleDirection in GetAvailableMovesForLocation(currentNode.Value, game))
                {
                    var possibleLocation = currentNode.Value + possibleDirection;
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

        //private void CalculateDistance(Game.Game game, CellLocation currentLocation,
        //                                int[,] shortestDistances,
        //                                bool[,] visited)
        //{
        //    var distanceToHere = shortestDistances[currentLocation];
        //    var availableMoves = GetAvailableMovesForLocation(currentLocation, game.Walls);

        //    foreach (var possibleDirection in availableMoves)
        //    {
        //        var newLocation = currentLocation + possibleDirection;
        //        if (!visited.ContainsKey(newLocation))
        //        {
        //            if (shortestDistances.TryGetValue(newLocation, out var previousDistance))
        //            {
        //                if (previousDistance > distanceToHere + 1)
        //                {
        //                    shortestDistances[newLocation] = distanceToHere + 1;
        //                }
        //            }
        //            else
        //            {
        //                shortestDistances[newLocation] = distanceToHere + 1;
        //            }
        //        }
        //    }

        //    //4
        //    visited[currentLocation] = true;

        //    foreach (var possibleDirection in availableMoves)
        //    {
        //        if (!visited.ContainsKey(currentLocation + possibleDirection))
        //        {
        //            CalculateDistance(game, currentLocation + possibleDirection, shortestDistances, visited);
        //        }
        //    }

        //}

        private List<Direction> GetAvailableMovesForLocation(CellLocation location, Game.Game game)
        {
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