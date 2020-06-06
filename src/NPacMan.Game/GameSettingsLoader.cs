using System;
using System.Collections.Generic;
using System.Linq;
using NPacMan.Game.GhostStrategies;
using NPacMan.Game.Properties;
using System.Text.Json;

namespace NPacMan.Game
{
    public class GameSettingsLoader
    {
        public static IGameSettings LoadFromFile(string filename)
        {
            var fileContents = System.IO.File.ReadAllText(filename);
            return Load(fileContents);
        }

        public static IGameSettings LoadFromResource()
        {
            var fileContents = Resources.Board.Replace("\r\n", Environment.NewLine);
            return Load(fileContents);
        }

        public static IGameSettings Load(string board)
        {
            var allRows = board.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var rows = allRows.Where(line => !line.StartsWith("{")).ToArray();
            var instructions = allRows.Where(line => line.StartsWith("{")).ToArray();

            var height = rows.Length;
            var width = 0;

            var ghostSetup = FindHomeLocations(instructions);

            var ghosts = ghostSetup.Select(data => 

                                        new Ghost(data.Name,
                                                 new CellLocation(data.StartingLocation.X, data.StartingLocation.Y),
                                                 Direction.Left,
                                                 new CellLocation(data.ScatterTarget.X, data.ScatterTarget.Y),
                                                 data.Name switch 
                                                 {
                                                     GhostNames.Blinky => new DirectToStrategy(new DirectToPacManLocation()),
                                                     GhostNames.Clyde => new DirectToStrategy(new StaysCloseToPacManLocation(GhostNames.Clyde)),
                                                     GhostNames.Inky => new DirectToStrategy(new InterceptPacManLocation(GhostNames.Blinky)),
                                                     GhostNames.Pinky => new DirectToStrategy(new DirectToExpectedPacManLocation()),
                                                     _ => new DirectToStrategy(new DirectToPacManLocation())
                                                 },
                                                 data.PillsToLeave)).ToList();

            var coins = new List<CellLocation>();
            var powerPills = new List<CellLocation>();
            var walls = new List<CellLocation>();
            var doors = new List<CellLocation>();
            var portalParts = new List<CellLocation>();
            var ghostHouse = new List<CellLocation>();
            CellLocation? fruit = null;
            PacMan? pacMan = null;
            for (int rowNumber = 0; rowNumber < height; rowNumber++)
            {
                var row = rows[rowNumber];
                for (int columnNumber = 0; columnNumber < row.Length; columnNumber++)
                {
                    var location = new CellLocation(columnNumber - 1, rowNumber);
                    switch (row[columnNumber])
                    {
                        case '▲':
                            pacMan = new PacMan(location, Direction.Up);
                            break;
                        case '▼':
                            pacMan = new PacMan(location, Direction.Down);
                            break;
                        case '►':
                            pacMan = new PacMan(location, Direction.Right);
                            break;
                        case '◄':
                            pacMan = new PacMan(location, Direction.Left);
                            break;
                        case 'X':
                            walls.Add(location);
                            break;
                        case '-':
                            doors.Add(location);
                            break;
                        case 'T':
                            portalParts.Add(location);
                            break;
                        case '.':
                            coins.Add(location);
                            break;
                        case '*':
                            powerPills.Add(location);
                            break;
                        case 'H':
                            ghostHouse.Add(location);
                            break;             
                        case 'F':
                            fruit = location;
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

            if (!fruit.HasValue)
                throw new Exception("Fruit seems to be missing from the board.");

            return new GameSettings(width - 2, height, walls, coins, powerPills, portals, pacMan, ghosts, doors, ghostHouse, fruit.Value);
        }

        class GhostSetup
        {

            public string Name { get; set; } = null!;

            public Location ScatterTarget { get; set; } = null!;

            public Location StartingLocation { get; set; } = null!;

            public int PillsToLeave { get; set; }
        }

        class Location
        {

            public int X { get; set; }
            public int Y { get; set; }
        }

        private static List<GhostSetup> FindHomeLocations(string[] instructions)
        {
            var result = new List<GhostSetup>();

            foreach (var instruction in instructions)
            {
                var setup = JsonSerializer.Deserialize<GhostSetup>(instruction, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                result.Add(setup);
            }

            return result;
        }

        private static ReadOnlySpan<char> ConsumeToAndEatToken(ReadOnlySpan<char> instruction, string token, out ReadOnlySpan<char> value)
        {
            var indexOf = instruction.IndexOf(token);
            if (indexOf < 0)
                throw new Exception($"String {instruction.ToString()} does not contain the expected token {token}");

            value = instruction[..(indexOf)];
            return instruction[(indexOf + 1)..];
        }

        private static ReadOnlySpan<char> EatToken(ReadOnlySpan<char> instruction, string token)
        {
            if (!instruction.StartsWith(token))
                throw new Exception($"String {instruction.ToString()} does not start with the expected token {token}");

            return instruction[token.Length..];
        }
    }
}