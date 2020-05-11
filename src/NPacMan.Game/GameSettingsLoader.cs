using System;
using System.Collections.Generic;
using System.Linq;

namespace NPacMan.Game
{
    public class GameSettingsLoader
    {
        public static IGameSettings LoadFromFile(string filename)
        {
            var fileContents = System.IO.File.ReadAllText(filename);
            return Load(fileContents);
        }
        public static IGameSettings Load(string board)
        {
            var allRows = board.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var rows = allRows.Where(line => !line.StartsWith("{")).ToArray();
            var instructions = allRows.Where(line => line.StartsWith("{")).ToArray();

            var height = rows.Length;
            var width = 0;

            var homeLocations = FindHomeLocations(instructions);

            var coins = new List<CellLocation>();
            var walls = new List<CellLocation>();
            var doors = new List<CellLocation>();
            var portalParts = new List<CellLocation>();
            var ghosts = new List<Ghost>();
            PacMan? pacMan = null;
            for (int rowNumber = 0; rowNumber < height; rowNumber++)
            {
                var row = rows[rowNumber];
                for (int columnNumber = 0; columnNumber < row.Length; columnNumber++)
                {
                    var location = new CellLocation(columnNumber - 1, rowNumber);
                    switch (row[columnNumber])
                    {
                        case 'B':
                            var homeB = homeLocations[GhostNames.Blinky];
                            ghosts.Add(new Ghost(GhostNames.Blinky,
                                                 location,
                                                 Direction.Left,
                                                 new CellLocation(homeB.X, homeB.Y),
                                                 new DirectToStrategy(new DirectToPacManLocation())));
                            break;
                        case 'P':
                            var homeP = homeLocations[GhostNames.Pinky];
                            ghosts.Add(new Ghost(GhostNames.Pinky,
                                                 location,
                                                 Direction.Left,
                                                 new CellLocation(homeP.X, homeP.Y),
                                                 new DirectToStrategy(new DirectToExpectedPacManLocation())));
                            break;
                        case 'I':
                            var homeI = homeLocations[GhostNames.Inky];
                            ghosts.Add(new Ghost(GhostNames.Inky,
                                                 location,
                                                  Direction.Left,
                                                 new CellLocation(homeI.X, homeI.Y),
                                                 new DirectToStrategy(new InterceptPacManLocation(GhostNames.Blinky))));
                            break;
                        case 'C':
                            var homeC = homeLocations[GhostNames.Clyde];
                            ghosts.Add(new Ghost(GhostNames.Clyde,
                                                 location,
                                                Direction.Left,
                                                 new CellLocation(homeC.X, homeC.Y),
                                                 new DirectToStrategy(new StaysCloseToPacManLocation(GhostNames.Clyde))));
                            break;
                        case '▲':
                            pacMan = new PacMan(location, Direction.Up, PacManStatus.Alive, 3);
                            break;
                        case '▼':
                            pacMan = new PacMan(location, Direction.Down, PacManStatus.Alive, 3);
                            break;
                        case '►':
                            pacMan = new PacMan(location, Direction.Right, PacManStatus.Alive, 3);
                            break;
                        case '◄':
                            pacMan = new PacMan(location, Direction.Left, PacManStatus.Alive, 3);
                            break;
                        case 'X':
                            walls.Add((columnNumber - 1, rowNumber));
                            break;
                        case '-':
                            doors.Add(location);
                            break;
                        case 'T':
                            portalParts.Add((columnNumber - 1, rowNumber));
                            break;
                        case '.':
                            coins.Add((columnNumber - 1, rowNumber));
                            break;
                        default:
                            break;
                    }

                    width = Math.Max(width, row.Length);
                }
            }

            if (portalParts.Count() != 0 && portalParts.Count() != 2)
                throw new Exception("Unexpected number of portals");

            var portals = new Dictionary<CellLocation, CellLocation>();
            if (portalParts.Any())
            {
                portals.Add(portalParts[0], portalParts[1]);
                portals.Add(portalParts[1], portalParts[0]);
            }

            if (pacMan is null)
                throw new Exception("Pacman seems to be missing from the board.");

            return new GameSettings(width - 2, height, walls, coins, portals, pacMan, ghosts, doors);
        }

        private static Dictionary<string, CellLocation> FindHomeLocations(string[] instructions)
        {
            var result = new Dictionary<string, CellLocation>();

            foreach (var instruction in instructions)
            {
                // Parses instructions in the form {Blinky=-10,-3}
                var span = EatToken(instruction.AsSpan(), "{");
                span = ConsumeToAndEatToken(span, "=", out var name);
                span = ConsumeToAndEatToken(span, ",", out var xString);
                span = ConsumeToAndEatToken(span, "}", out var yString);

                if (!int.TryParse(xString, out var x))
                    throw new Exception($"{xString.ToString()} could not be parsed as a numeric value for the ghosts home X location.");

                if (!int.TryParse(yString, out var y))
                    throw new Exception($"{yString.ToString()} could not be parsed as a numeric value for the ghosts home Y location.");

                result.Add(name.ToString(), new CellLocation(x, y));

            }

            return result;
        }

        private static ReadOnlySpan<char> ConsumeToAndEatToken(ReadOnlySpan<char> instruction, string token, out ReadOnlySpan<char> value)
        {
            var indexOf = instruction.IndexOf(token);
            if (indexOf < 0)
                throw new Exception($"String {instruction.ToString()} does not contain the expected token {token}");

            value = instruction[..(indexOf)];
            return instruction[(indexOf+1)..];
        }

        private static ReadOnlySpan<char> EatToken(ReadOnlySpan<char> instruction, string token)
        {
            if (!instruction.StartsWith(token))
                throw new Exception($"String {instruction.ToString()} does not start with the expected token {token}");

            return instruction[token.Length..];
        }
    }
}