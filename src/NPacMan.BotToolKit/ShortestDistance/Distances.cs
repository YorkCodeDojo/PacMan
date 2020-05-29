using System.Linq;

namespace NPacMan.BotSDK
{
    public class Distances
    {
        private readonly int[,] _shortestDistances;
        private readonly Graph _graph;

        internal Distances(int[,] shortestDistances, Graph graph)
        {
            _shortestDistances = shortestDistances;
            _graph = graph;
        }

        /// <summary>
        ///     Returns the shortest distance from here to the target location
        /// </summary>
        /// <param name="targetLocation"></param>
        /// <returns></returns>
        public int DistanceTo(CellLocation targetLocation)
        {
            return _shortestDistances[targetLocation.X, targetLocation.Y];
        }

        /// <summary>
        ///     Returns an array of all the directions we need to walk in order to
        ///     walk directly to the target location.
        /// </summary>
        /// <param name="targetLocation"></param>
        /// <returns></returns>
        public Direction[] RouteToTarget(CellLocation targetLocation)
        {
            var bestDirection = default(Direction);
            var bestNewNode = default(LinkedCell);
            var distanceToCoin = DistanceTo(targetLocation);
            var currentNode = _graph.GetNodeForLocation(targetLocation);
            var result = new Direction[distanceToCoin];
            while (distanceToCoin > 0)
            {
                var bestScore = int.MaxValue;
                foreach (var (possibleDirection, possibleLocation) in currentNode!.AvailableMoves)
                {
                    if (DistanceTo(possibleLocation.Location) < bestScore)
                    {
                        bestScore = DistanceTo(possibleLocation.Location);
                        bestDirection = possibleDirection;
                        bestNewNode = possibleLocation;
                    }
                }

                currentNode = bestNewNode!;
                distanceToCoin--;

                result[distanceToCoin] = bestDirection.Opposite();
            }

            return result;
        }

        /// <summary>
        ///     Returns the initial direction we need to turn in order to walk directly to the target location.
        /// </summary>
        /// <param name="targetLocation"></param>
        /// <returns></returns>
        public Direction DirectionToTarget(CellLocation targetLocation) => RouteToTarget(targetLocation).First();
    }
}
