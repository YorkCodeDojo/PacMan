namespace NPacMan.BotSDK
{
    public class Distances
    {
        private readonly int[,] _shortestDistances;

        internal Distances(int[,] shortestDistances)
        {
            _shortestDistances = shortestDistances;
        }

        public int DistanceTo(CellLocation location)
        {
            return _shortestDistances[location.X, location.Y];
        }
    }
}
