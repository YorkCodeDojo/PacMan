using System;

namespace NPacMan.BotSDK
{
    public static class DistancesCalculator
    {
        public static Distances CalculateDistances(BotGame game, Graph graph, LinkedCell currentLocation)
        {
            //1
            var width = game.Board.Width;
            var height = game.Board.Height;

            var shortestDistances = new int[width, height];
            var visited = new bool[width, height];
            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
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
                var linkedCell = graph.GetNodeForLocation(currentNode.Value);

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
                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < width; c++)
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

            return new Distances(shortestDistances);
        }
    }
}
