using System;
using System.Collections.Generic;
using System.Linq;

namespace NPacMan.Game
{
    public class GameSettingsLoader
    {
        public static IGameSettings Load(string board)
        {
            var rows = board.Split(new []{Environment.NewLine}, StringSplitOptions.None);
            var height = rows.Length;
            var width = 0;

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
                            ghosts.Add(new Ghost("Blinky", columnNumber - 1, rowNumber, new DirectChaseToPacManStrategy()));
                            break;
                        case 'P':
                            ghosts.Add(new Ghost("Pinky", columnNumber - 1, rowNumber, new StandingStillGhostStrategy()));
                            break;
                        case 'I':
                            ghosts.Add(new Ghost("Inky", columnNumber - 1, rowNumber, new StandingStillGhostStrategy()));
                            break;
                        case 'C':
                            ghosts.Add(new Ghost("Clyde", columnNumber - 1, rowNumber, new StandingStillGhostStrategy()));
                            break;
                        case '▲':
                            pacMan = new PacMan(columnNumber-1, rowNumber, Direction.Up, PacManStatus.Alive, 3);
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
                            walls.Add((columnNumber-1, rowNumber));
                            break;
                        case '.':
                            coins.Add((columnNumber-1,rowNumber));
                            break;
                        case 'T':
                            portalParts.Add((columnNumber-1, rowNumber));
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

            return new GameSettings(width-2, height, walls, coins, portals, pacMan, ghosts);
        }
    }
}