using System;
using System.Collections.Generic;

namespace NPacMan.SharedUi.Map
{
    /// <summary>
    /// Checks for patterns in the given cell (and it's surrounding cells) and sets the detailed map piece
    /// </summary>
    internal class CellPatternFinder
    {
        private readonly List<CellPattern> _patterns;

        public CellPatternFinder()
        {
            _patterns = new List<CellPattern>
            {
                new CellPattern("-0-GGD---", BoardPiece.GhostEndLeft),
                new CellPattern("-0-DGG---", BoardPiece.GhostEndRight),
                new CellPattern("-0-GGG---", BoardPiece.DoubleBottom),
                new CellPattern("---GGG-0-", BoardPiece.DoubleTop),
                new CellPattern("-G-0G--G-", BoardPiece.DoubleRight),
                new CellPattern("-G--G0-G-", BoardPiece.DoubleLeft),
                new CellPattern("-0-0GG-G-", BoardPiece.GhostTopLeft),
                new CellPattern("-0-GG0-G-", BoardPiece.GhostTopRight),
                new CellPattern("-G-0GG-0-", BoardPiece.GhostBottomLeft),
                new CellPattern("-G-GG0-0-", BoardPiece.GhostBottomRight),
                new CellPattern("-0-WXW---", BoardPiece.Top),
                new CellPattern("---WXW-0-", BoardPiece.Bottom),
                new CellPattern("-W-0X--W-", BoardPiece.Left),
                new CellPattern("-W--X0-W-", BoardPiece.Right),
                new CellPattern("-0-WX0---", BoardPiece.TopRight),
                new CellPattern("-0-0XW---", BoardPiece.TopLeft),
                new CellPattern("---WX0-0-", BoardPiece.BottomRight),
                new CellPattern("---0XW-0-", BoardPiece.BottomLeft),
                new CellPattern("-W0-XW---", BoardPiece.InnerBottomLeft),
                new CellPattern("0W-WX----", BoardPiece.InnerBottomRight),
                new CellPattern("----XW-W0", BoardPiece.InnerTopLeft),
                new CellPattern("---WX-0W-", BoardPiece.InnerTopRight),
                new CellPattern("-0-###---", BoardPiece.DoubleBottom),
                new CellPattern("---###-0-", BoardPiece.DoubleTop),
                new CellPattern("-#-0#--#-", BoardPiece.DoubleRight),
                new CellPattern("-#--#0-#-", BoardPiece.DoubleLeft),
                new CellPattern("-~-~##-0-", BoardPiece.DoubleTop),
                new CellPattern("-~-##~-0-", BoardPiece.DoubleTop),
                new CellPattern("-0-~##-~-", BoardPiece.DoubleBottom),
                new CellPattern("-0-##~-~-", BoardPiece.DoubleBottom),
                new CellPattern("-00##0-#0", BoardPiece.TopRight),
                new CellPattern("00-0##0#-", BoardPiece.TopLeft),
                new CellPattern("-#0##0-00", BoardPiece.BottomRight),
                new CellPattern("0#-0##00-", BoardPiece.BottomLeft),
                new CellPattern("-0-##0---", BoardPiece.DoubleTopRight),
                new CellPattern("-0-0##---", BoardPiece.DoubleTopLeft),
                new CellPattern("---##0-0-", BoardPiece.DoubleBottomRight),
                new CellPattern("---0##-0-", BoardPiece.DoubleBottomLeft),
                new CellPattern("-#0-##---", BoardPiece.DoubleBottomLeft),
                new CellPattern("0#-##----", BoardPiece.DoubleBottomRight),
                new CellPattern("----##-#0", BoardPiece.DoubleTopLeft),
                new CellPattern("---##-0#-", BoardPiece.DoubleTopRight),
                new CellPattern("----##-X0", BoardPiece.JoinTopRight),
                new CellPattern("---##-0X-", BoardPiece.JoinTopLeft),
                new CellPattern("-#0-#X---", BoardPiece.JoinLeftHandTop),
                new CellPattern("0#-X#----", BoardPiece.JoinRightHandTop),
                new CellPattern("----#X-#0", BoardPiece.JoinLeftHandBottom),
                new CellPattern("---X#-0#-", BoardPiece.JoinRightHandBottom),
                new CellPattern("-X0-##---", BoardPiece.JoinBottomRight),
                new CellPattern("0X-##----", BoardPiece.JoinBottomLeft)
            };
        }

        public BoardPiece FindBoardPiece(MapCellDetail board)
        {
            foreach (var pattern in _patterns)
            {
                if (pattern.DoCellsMatchPattern(board))
                    return pattern.BoardPiece;
            }

            return BoardPiece.Undefined;

            // Failed to match - loop through again to help debug layout

//#if DEBUG

//            foreach (var pattern in _patterns)
//            {
//                if (pattern.DoCellsMatchPattern(board))
//                    return pattern.BoardPiece;
//            }
//#endif

//            throw new Exception($"No matching board piece pattern @ c{board.X},{board.Y}");
        }
    }
}