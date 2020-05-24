using System.Linq;

namespace NPacMan.BotSDK
{
    public static class GraphBuilder
    {
        public static Graph Build(BotGame game)
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

            var portals = game.Portals.ToDictionary(p => p.Entry, p => p.Exit);

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
                        else if (portals.TryGetValue(location.Above, out var otherSideOfPortal))
                        {
                            var exit = otherSideOfPortal + Direction.Up;
                            above = cells[exit.X, exit.Y];
                        }

                        LinkedCell? below = null;
                        if (!game.Walls.Contains(location.Below) && location.Y < game.Height - 1)
                        {
                            below = cells[location.X, location.Y + 1];
                        }
                        else if (portals.TryGetValue(location.Below, out var otherSideOfPortal))
                        {
                            var exit = otherSideOfPortal + Direction.Down;
                            below = cells[exit.X, exit.Y];
                        }

                        LinkedCell? left = null;
                        if (!game.Walls.Contains(location.Left) && location.X > 0)
                        {
                            left = cells[location.X - 1, location.Y];
                        }
                        else if (portals.TryGetValue(location.Left, out var otherSideOfPortal))
                        {
                            var exit = otherSideOfPortal + Direction.Left;
                            left = cells[exit.X, exit.Y];
                        }

                        LinkedCell? right = null;
                        if (!game.Walls.Contains(location.Right) && location.X < game.Width - 1)
                        {
                            right = cells[location.X + 1, location.Y];
                        }
                        else if (portals.TryGetValue(location.Right, out var otherSideOfPortal))
                        {
                            var exit = otherSideOfPortal + Direction.Right;
                            right = cells[exit.X, exit.Y];
                        }


                        cells[column, row].DefineNeighbours(left, right, above, below);
                    }
                }
            }

            return new Graph(cells);
        }
    }
}
