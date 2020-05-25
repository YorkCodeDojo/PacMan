namespace NPacMan.SharedUi.Map
{
    /// <summary>
    /// Holds the detail of one cell of the map
    /// </summary>
    internal class MapCellDetail
    {
        public readonly int Y;
        public readonly int X;
        public BasicMapPiece BasicType;
        public BoardPiece Piece;
        public bool Filled;

        private readonly MapCellDetail[,] _map;

        public bool IsWall => BasicType != BasicMapPiece.Space;

        public MapCellDetail(MapCellDetail[,] map, int x, int y, BasicMapPiece basicType)
        {
            _map = map;
            Y = y;
            X = x;
            BasicType = basicType;
        }

        public MapCellDetail CellAbove => Cell(0, -1);
        public MapCellDetail CellBelow => Cell(0, 1);
        public MapCellDetail CellLeft => Cell(-1, 0);
        public MapCellDetail CellRight => Cell(1, 0);
        public MapCellDetail CellTopLeft => Cell(-1, -1);
        public MapCellDetail CellTopRight => Cell(1, -1);
        public MapCellDetail CellBottomLeft => Cell(-1, 1);
        public MapCellDetail CellBottomRight => Cell(1, 1);

        public MapCellDetail Cell(int directionColumn, int directionRow) =>
            _map[X + directionColumn, Y + directionRow];

        public bool IsSpace => BasicType == BasicMapPiece.Space;
    }
}