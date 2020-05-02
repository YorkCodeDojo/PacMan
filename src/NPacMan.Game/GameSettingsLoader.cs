using System;
using System.Collections.Generic;
using System.Linq;

namespace NPacMan.Game
{
    public class GameSettingsLoader
    {
        public static IGameSettings Load(string board)
        {
            var rows = board.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var height = rows.Length;
            var width = 0;

            var homeLocations = FindHomeLocations(width, height, rows);

            var coins = new List<(int, int)>();
            var walls = new List<(int, int)>();
            var portalParts = new List<(int, int)>();
            var ghosts = new List<Ghost>();
            PacMan? pacMan = null;
            for (int rowNumber = 0; rowNumber < height; rowNumber++)
            {
                var row = rows[rowNumber];
                for (int columnNumber = 0; columnNumber < row.Length; columnNumber++)
                {
                    switch (row[columnNumber])
                    {
                        case 'B':
                            var homeB = homeLocations['b'];
                            ghosts.Add(new Ghost(GhostNames.Blinky, columnNumber - 1, rowNumber, new DirectChaseToPacManStrategy(), new MoveHomeGhostStrategy(homeB.X, homeB.Y)));
                            break;
                        case 'P':
                            var homeP = homeLocations['p'];
                            ghosts.Add(new Ghost(GhostNames.Pinky, columnNumber - 1, rowNumber, new StandingStillGhostStrategy(), new MoveHomeGhostStrategy(homeP.X, homeP.Y)));
                            break;
                        case 'I':
                            var homeI = homeLocations['i'];
                            ghosts.Add(new Ghost(GhostNames.Inky, columnNumber - 1, rowNumber, new StandingStillGhostStrategy(), new MoveHomeGhostStrategy(homeI.X, homeI.Y)));
                            break;
                        case 'C':
                            var homeC = homeLocations['c'];
                            ghosts.Add(new Ghost(GhostNames.Clyde, columnNumber - 1, rowNumber, new StandingStillGhostStrategy(), new MoveHomeGhostStrategy(homeC.X, homeC.Y)));
                            break;
                        case '▲':
                            pacMan = new PacMan(columnNumber - 1, rowNumber, Direction.Up, PacManStatus.Alive, 3);
                            break;
                        case '▼':
                            pacMan = new PacMan(columnNumber - 1, rowNumber, Direction.Down, PacManStatus.Alive, 3);
                            break;
                        case '►':
                            pacMan = new PacMan(columnNumber - 1, rowNumber, Direction.Right, PacManStatus.Alive, 3);
                            break;
                        case '◄':
                            pacMan = new PacMan(columnNumber - 1, rowNumber, Direction.Left, PacManStatus.Alive, 3);
                            break;
                        case 'X':
                            walls.Add((columnNumber - 1, rowNumber));
                            break;
                        case '.':
                            coins.Add((columnNumber - 1, rowNumber));
                            break;
                        case 'T':
                            portalParts.Add((columnNumber - 1, rowNumber));
                            break;
                        default:
                            break;
                    }

                    width = Math.Max(width, row.Length);
                }
            }

            if (portalParts.Count() != 0 && portalParts.Count() != 2)
                throw new Exception("Unexpected number of portals");

            var portals = new Dictionary<(int, int), (int, int)>();
            if (portalParts.Any())
            {
                portals.Add(portalParts[0], portalParts[1]);
                portals.Add(portalParts[1], portalParts[0]);
            }

            if (pacMan is null)
                throw new Exception("Pacman seems to be missing from the board.");

            return new GameSettings(width - 2, height, walls, coins, portals, pacMan, ghosts);
        }

        private static Dictionary<char,(int X, int Y)> FindHomeLocations(int width, int height, string[] rows)
        {
            var result = new Dictionary<char, (int X, int Y)>();
            for (int rowNumber = 0; rowNumber < height; rowNumber++)
            {
                var row = rows[rowNumber];
                for (int columnNumber = 0; columnNumber < row.Length; columnNumber++)
                {
                    var c = row[columnNumber];
                    if (char.IsLower(c))
                    {
                        result.Add(c, (columnNumber - 1, rowNumber));
                    }
                }
            }
            return result;
        }
    }
}