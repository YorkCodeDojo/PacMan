using System;
using System.Linq;
using System.Text;

namespace NPacMan.Game.Tests
{
    class AsciiTestSettingBuilder
    {
        private readonly TestGameSettings _gameSettings;
        private CellLocation _topLeft;

        public AsciiTestSettingBuilder(TestGameSettings gameSettings)
        {
            _gameSettings = gameSettings;
        }
        internal void CreateBoard(params string[] rows)
        {
            rows = rows.Select(RemovePadding).ToArray();

            _topLeft = CalculateTopLeftFromPacMan(rows);

            RemoveCurrentSettings();

            var currentCell = _topLeft;
            foreach (var row in rows)
            {
                CreateRow(row, currentCell);
                currentCell = currentCell.Below;
            }
        }

        internal void AssertBoard(Game game, params string[] rows)
        {
            rows = rows.Select(RemovePadding).ToArray();

            var currentCell = _topLeft;
            foreach (var row in rows)
            {
                AssertRow(game, row, currentCell);
                currentCell = currentCell.Below;
            }
        }

        private void RemoveCurrentSettings()
        {
            _gameSettings.GhostHouse.Clear();
        }

        private CellLocation CalculateTopLeftFromPacMan(string[] rows)
        {
            var pacManOffset = FindPacMan(rows);
            var currentCell = _gameSettings.PacMan.Location;

            for (int i = 0; i < pacManOffset.X; i++)
                currentCell = currentCell.Left;

            for (int i = 0; i < pacManOffset.Y; i++)
                currentCell = currentCell.Above;
            return currentCell;
        }



        private string RemovePadding(string row)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < row.Length; i += 2)
                sb.Append(row[i]);
            return sb.ToString();
        }

        private CellLocation FindPacMan(string[] rows)
        {
            var y = 0;
            foreach (var row in rows)
            {
                var x = 0;
                foreach (var cell in row)
                {
                    if (cell == 'P')
                        return new CellLocation(x, y);

                    x++;
                }

                y++;
            }

            throw new Exception("Could not find pac man");
        }

        private void CreateRow(string definition, CellLocation startingFrom)
        {
            var currentCell = startingFrom;
            foreach (var c in definition)
            {
                switch (c)
                {
                    case '.':
                        _gameSettings.Coins.Add(currentCell);
                        break;
                    case '*':
                        _gameSettings.PowerPills.Add(currentCell);
                        break;
                    case '#':
                        _gameSettings.Walls.Add(currentCell);
                        break;
                    case '-':
                        _gameSettings.Doors.Add(currentCell);
                        break;
                    case 'H':
                        _gameSettings.GhostHouse.Add(currentCell);
                        break;
                    case 'G':
                        if (!_gameSettings.Ghosts.Any(g => g.Location == currentCell))
                        {
                            throw new Exception("There is no ghost at this location.");
                        }
                        break;
                }
                currentCell = currentCell.Right;
            }
        }

        private void AssertRow(Game game, string definition, CellLocation startingFrom)
        {
            var currentCell = startingFrom;
            foreach (var c in definition)
            {
                switch (c)
                {
                    case '.':
                        if (!game.Coins.Contains(currentCell))
                        {
                            throw new Exception($"Cell {currentCell} is meant to contain a coin, but currently contains {NameSymbol(c)}");
                        }
                        break;
                    case '*':
                        if (!game.PowerPills.Contains(currentCell))
                        {
                            throw new Exception($"Cell {currentCell} is meant to contain a powerpill, but currently contains {NameSymbol(c)}");
                        }
                        break;
                    case '#':
                        if (!game.Walls.Contains(currentCell))
                        {
                            throw new Exception($"Cell {currentCell} is meant to contain a wall, but currently contains {NameSymbol(c)}");
                        }
                        break;
                    case '-':
                        if (!game.Doors.Contains(currentCell))
                        {
                            throw new Exception($"Cell {currentCell} is meant to contain a door, but currently contains {NameSymbol(c)}");
                        }
                        break;
                    case 'H':
                        if (!game.GhostHouse.Contains(currentCell))
                        {
                            throw new Exception($"Cell {currentCell} is meant to contain a ghost house, but currently contains {NameSymbol(c)}");
                        }
                        break;
                    case 'G':
                        if (!game.Ghosts.Values.Any(g => g.Location == currentCell))
                        {
                            throw new Exception($"Cell {currentCell} is meant to contain a ghost, but currently contains {NameSymbol(c)}");
                        }
                        break;
                    case 'P':
                        if (game.PacMan.Location != currentCell)
                        {
                            throw new Exception($"PacMan is meant to be at {currentCell} but is actually at {game.PacMan.Location}.");
                        }
                        break;
                }
                currentCell = currentCell.Right;
            }
        }

        private string NameSymbol(char c)
        {
            return c switch
            {
                '.' => "a coin",
                '*' => "a powerpill",
                '#' => "a wall",
                '_' => "a door",
                'H' => "a ghost house",
                'G' => "a ghost",
                'P' => "PacMan",
                _ => c.ToString()
            };
        }
    }
}
