namespace NPacMan.BotSDK
{
    public class Graph
    {
        private readonly LinkedCell[,] _cells;

        internal Graph(LinkedCell[,] cells)
        {
            _cells = cells;
        }

        public LinkedCell GetNodeForLocation(CellLocation location)
        {
            return _cells[location.X, location.Y];
        }
    }
}
