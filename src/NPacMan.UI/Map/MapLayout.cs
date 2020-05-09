using System;
using System.Collections.Generic;
using System.Linq;
using NPacMan.Game;

namespace NPacMan.UI.Map
{
    public class MapLayout
    {
        public int DisplayHeight => _height - 2;
        public int DisplayWidth => _width - 2;

        private int _width;
        private int _height;

        private MapCellDetail[,] _map;

        private readonly CellPatternFinder _cellPatternFinder;

        public MapLayout()
        {
            _map = new MapCellDetail[2, 2];
            _width = 2;
            _height = 2;

            _cellPatternFinder = new CellPatternFinder();
        }

        public void UpdateFromGame(Game.Game game)
        {
            // The matrix is created with a blank row round the whole map
            // This is used to find the double walls
            // It also naturally protects from array bounds checking

            _height = game.Height + 2;
            _width = game.Width + 2;
            _map = new MapCellDetail[_width, _height];

            var doors = game.Doors;

            GetWallsFromGame(game, doors);

            var validPlayCell = FindValidPlayCell();
            MakeBasicMap(validPlayCell.X, validPlayCell.Y, doors);

            MakeDetailedMap();
        }

        /// <summary>
        /// Find the play area by looking for the inside edge of a corner
        /// </summary>
        /// <returns></returns>
        private CellLocation FindValidPlayCell()
        {
            for (int y = 1; y < _height - 1; y++)
            {
                for (int x = 1; x < _width - 1; x++)
                {
                    var cell = _map[x, y];
                    if (cell.IsSpace && cell.CellAbove.IsWall && cell.CellLeft.IsWall)
                        return new CellLocation(x, y);
                }
            }

            throw new Exception("Can't find play area");
        }

        /// <summary>
        /// Create a matrix of map pieces from Game object
        /// </summary>
        /// <param name="game"></param>
        private void GetWallsFromGame(Game.Game game, IReadOnlyCollection<CellLocation> doors)
        {
            // Populate the map matrix from the given list of walls
            
            foreach (var item in game.Walls)
            {
                var x = item.X + 1;
                var y = item.Y + 1;
                _map[x, y] = new MapCellDetail(_map, x, y, BasicMapPiece.SingleWall);
            }

            foreach (var item in doors)
            {
                var x = item.X + 1;
                var y = item.Y + 1;
                _map[x, y] = new MapCellDetail(_map, x, y, BasicMapPiece.Door);
            }

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_map[x, y] == null)
                        _map[x, y] = new MapCellDetail(_map, x, y, BasicMapPiece.Space);
                }
            }
        }
        
        /// <summary>
        /// Convert the basic walls into singles/doubles etc.
        /// </summary>
        private void MakeBasicMap(int playX, int playY, IReadOnlyCollection<CellLocation> doors)
        {
            foreach (var door in doors)
            {
                TraceGhostHouseWalls(_map[door.X + 1, door.Y + 1]);
            }

            BlockTunnels();
            
        //    ChangeSingleWallTouching(BasicMapPiece.InnerSpace, BasicMapPiece.GhostWall);

            Flood(_map[playX, playY], BasicMapPiece.PlayArea);
            Flood(_map[0, 0], BasicMapPiece.OuterSpace);
            Flood(_map[0, _height - 1], BasicMapPiece.OuterSpace);
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    var cell = _map[x, y];
                    if (cell.IsSpace)
                        cell.BasicType = BasicMapPiece.InnerSpace;
                }
            }
            ChangeSingleWallTouching(BasicMapPiece.OuterSpace, BasicMapPiece.DoubleWall);
        }

        /// <summary>
        /// Tunnels have incomplete walls. Block them off.
        /// </summary>
        private void BlockTunnels()
        {

            for (int y = 1; y < _height - 1; y++)
            {
                for (int x = 1; x < _width - 1; x++)
                {
                    var cell = _map[x, y];
                    if (cell.IsWall)
                    {
                        var pattern = TunnelPattern(cell);
                        if (pattern == 'H')
                        {
                            if (y > 2)
                            {
                                var gap = cell.CellAbove;
                                if (gap.IsSpace && TunnelPattern(gap.CellAbove) == 'H')
                                {
                                    gap.BasicType = BasicMapPiece.Portal;
                                }
                            }

                            if (y < _height - 2)
                            {
                                var gap = cell.CellBelow;
                                if (gap.IsSpace && TunnelPattern(gap.CellAbove) == 'H')
                                {
                                    gap.BasicType = BasicMapPiece.Portal;
                                }
                            }
                        }

                        if (pattern == 'V')
                        {
                            if (x > 2)
                            {
                                var gap = cell.CellLeft;
                                if (gap.IsSpace && TunnelPattern(gap.CellLeft) == 'V')
                                {
                                    gap.BasicType = BasicMapPiece.Portal;
                                }
                            }

                            if (x < _width - 2)
                            {
                                var gap = cell.CellRight;
                                if (gap.IsSpace && TunnelPattern(gap.CellRight) == 'V')
                                {
                                    gap.BasicType = BasicMapPiece.Portal;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Find the pattern
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private char TunnelPattern(MapCellDetail cell)
        {
            var pattern = (cell.CellAbove.IsWall ? "V" : "")
                   + (cell.CellBelow.IsWall ? "V" : "")
                   + (cell.CellLeft.IsWall ? "H" : "")
                   + (cell.CellRight.IsWall ? "H" : "");
            return pattern.Length == 1 ? pattern[0] : ' ';
        }

        /// <summary>
        /// Find the ghost walls by starting from a ghost door and tracing
        /// </summary>
        private void TraceGhostHouseWalls(MapCellDetail cell)
        {
            if (cell.Filled || (cell.BasicType!=BasicMapPiece.SingleWall && cell.BasicType!=BasicMapPiece.Door)) return;

            if (cell.BasicType == BasicMapPiece.SingleWall)
            {
                cell.BasicType = BasicMapPiece.GhostWall;
            }

            cell.Filled = true;
            TraceGhostHouseWalls(cell.CellAbove);
            TraceGhostHouseWalls(cell.CellBelow);
            TraceGhostHouseWalls(cell.CellLeft);
            TraceGhostHouseWalls(cell.CellRight);
        }

        /// <summary>
        /// Change all single walls that are touching the given type to the new wall type
        /// </summary>
        /// <param name="touchType"></param>
        /// <param name="wallType"></param>
        private void ChangeSingleWallTouching(BasicMapPiece touchType, BasicMapPiece wallType)
        {
            for (int y = 1; y < _height - 1; y++)
            {
                for (int x = 1; x < _width - 1; x++)
                {
                    var cell = _map[x, y];
                    if (cell.BasicType == BasicMapPiece.SingleWall)
                    {
                        if (cell.CellAbove.BasicType == touchType ||
                            cell.CellBelow.BasicType == touchType ||
                            cell.CellLeft.BasicType == touchType ||
                            cell.CellRight.BasicType == touchType ||
                            cell.CellTopLeft.BasicType == touchType ||
                            cell.CellTopRight.BasicType == touchType ||
                            cell.CellBottomLeft.BasicType == touchType ||
                            cell.CellBottomRight.BasicType == touchType)
                        {
                            cell.BasicType = wallType;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Flood fill the matrix with the given type
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="type"></param>
        private void Flood(MapCellDetail cell, BasicMapPiece type)
        {
            if (cell.Filled || !cell.IsSpace) return;
            cell.Filled = true;
            cell.BasicType = type;

            if (cell.X > 0) Flood(cell.CellLeft, type);
            if (cell.X < _width - 1) Flood(cell.CellRight, type);
            if (cell.Y > 0) Flood(cell.CellAbove, type);
            if (cell.Y < _height - 1) Flood(cell.CellBelow, type);
        }

        /// <summary>
        /// Get the Board Piece to show at the given X and Y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public BoardPiece BoardPieceToDisplay(int x, int y) => _map[x + 1, y + 1].Piece;

        /// <summary>
        /// Convert the basic wall types into corners etc.
        /// </summary>
        private void MakeDetailedMap()
        {

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    var cell = _map[x, y];
                    switch (cell.BasicType)
                    {
                        case BasicMapPiece.Space:
                        case BasicMapPiece.InnerSpace:
                        case BasicMapPiece.OuterSpace:
                        case BasicMapPiece.PlayArea:
                        case BasicMapPiece.Portal:
                            cell.Piece = BoardPiece.Blank;
                            break;
                        case BasicMapPiece.Door:
                            cell.Piece = BoardPiece.GhostDoor;
                            break;
                        default:
                            cell.Piece = _cellPatternFinder.FindBoardPiece(cell);
                            break;
                    }
                }
            }
        }

#if DEBUG

        // These are tools to help with debugging a map design
        // Can check for basic map and detailed map with all pieces to see all has parsed correctly

        /// <summary>
        /// To help with debugging basic map
        /// </summary>
        public string[] ToMapStringBasic => ConvertMapToStringArray(cell => (int)cell.BasicType);

        /// <summary>
        /// To help with debugging detailed map
        /// </summary>
        public string[] ToMapStringPieces => ConvertMapToStringArray(cell => (int)cell.Piece);

        /// <summary>
        /// Produces a copy of the processed map as a string array
        /// To help with debugging map parsing
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        private string[] ConvertMapToStringArray(Func<MapCellDetail, int> func)
        {
            var map = new string[_height];
            for (int y = 0; y < _height; y++)
            {
                var st = "";
                for (int x = 0; x < _width; x++)
                {
                    st = st + (char)('A' + func(_map[x, y]));
                }

                map[y] = st;
            }

            return map;
        }
#endif
    }
}
