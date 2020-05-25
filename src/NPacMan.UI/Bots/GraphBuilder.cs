﻿using NPacMan.Game;
using System;
using System.Linq;

namespace NPacMan.UI.Bots
{
    internal static class GraphBuilder
    {
        public static LinkedCell[,] Build(Game.Game game)
        {
            var cells = new LinkedCell[game.Width, game.Height];

            for (int row = 0; row < game.Height; row++)
            {
                for (int column = 0; column < game.Width; column++)
                {
                    if (!game.Walls.Contains((column, row)))
                    {
                        cells[column, row] = new LinkedCell((column, row));
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
                            above = cells[location.X, location.Y - 1];
                        }
                        else if (game.Portals.TryGetValue(location.Above, out var otherSideOfPortal))
                        {
                            var exit = otherSideOfPortal + Direction.Up;
                            above = cells[exit.X, exit.Y];
                        }

                        LinkedCell? below = null;
                        if (!game.Walls.Contains(location.Below) && location.Y < game.Height - 1)
                        {
                            below = cells[location.X, location.Y + 1];
                        }
                        else if (game.Portals.TryGetValue(location.Below, out var otherSideOfPortal))
                        {
                            var exit = otherSideOfPortal + Direction.Down;
                            below = cells[exit.X, exit.Y];
                        }

                        LinkedCell? left = null;
                        if (!game.Walls.Contains(location.Left) && location.X > 0)
                        {
                            left = cells[location.X - 1, location.Y];
                        }
                        else if (game.Portals.TryGetValue(location.Left, out var otherSideOfPortal))
                        {
                            var exit = otherSideOfPortal + Direction.Left;
                            left = cells[exit.X, exit.Y];
                        }

                        LinkedCell? right = null;
                        if (!game.Walls.Contains(location.Right) && location.X < game.Width - 1)
                        {
                            right = cells[location.X + 1, location.Y];
                        }
                        else if (game.Portals.TryGetValue(location.Right, out var otherSideOfPortal))
                        {
                            var exit = otherSideOfPortal + Direction.Right;
                            right = cells[exit.X, exit.Y];
                        }


                        cells[column, row].DefineNeighbours(left, right, above, below);
                    }
                }
            }

            return cells;
        }
    }
}