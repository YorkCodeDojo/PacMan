namespace NPacMan.UI.Map
{
    /// <summary>
    /// A pattern of 9 cells around a target cell that correspond to a particular map piece
    /// </summary>
    public class CellPattern
    {
        public readonly BoardPiece BoardPiece;
        private readonly string[] _pattern;

        public CellPattern(string pattern, BoardPiece piece)
        {
            BoardPiece = piece;
            _pattern = new string[3];
            for (int y = 0; y < 3; y++)
            {
                _pattern[y] = pattern.Substring(y * 3, 3);
            }
        }

        private char PatternCell(int x, int y) => _pattern[y + 1][x + 1];

        /// <summary>
        /// Check the nine squares around the given cell for matching with pattern
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public bool DoCellsMatchPattern(MapCellDetail board)
        {
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    var onBoard = board.Cell(x, y);
                    var compare = PatternCell(x, y);

                    if (compare == 'G' && onBoard.BasicType != BasicMapPiece.GhostWall) return false;

                    if (compare == 'D' && onBoard.BasicType != BasicMapPiece.Door) return false;

                    if (compare == 'X' && onBoard.BasicType != BasicMapPiece.SingleWall) return false;

                    if (compare == 'W' && onBoard.BasicType != BasicMapPiece.SingleWall
                                       && onBoard.BasicType != BasicMapPiece.DoubleWall
                                       && onBoard.BasicType != BasicMapPiece.Portal) return false;

                    if (compare == '0' && onBoard.BasicType != BasicMapPiece.Space
                                       && onBoard.BasicType != BasicMapPiece.PlayArea
                                       && onBoard.BasicType != BasicMapPiece.Portal) return false;

                    if (compare == '#' && onBoard.BasicType != BasicMapPiece.DoubleWall) return false;

                    if (compare == '~' && onBoard.BasicType != BasicMapPiece.OuterSpace) return false;
                }
            }

            return true;
        }
    }
}