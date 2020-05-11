using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NPacMan.Game;

namespace NPacMan.ConsoleApp
{
    class Program
    {
        private static CellLocation _lastPacManLocation;
        private static IReadOnlyCollection<CellLocation> _allCoins = new List<CellLocation>();

        static async Task Main(string[] args)
        {
            var game = new Game.Game(new GameClock(), GameSettingsLoader.LoadFromFile("board.txt"));
            _allCoins = game.Coins;
            while (true)
            {
                var keys = GetAllKeysPressed();

                if (keys.Contains(ConsoleKey.Escape))
                    break;
                
                var directions = keys.Select(x => x switch
                {
                    ConsoleKey.UpArrow => Direction.Up,
                    ConsoleKey.DownArrow => Direction.Down,
                    ConsoleKey.LeftArrow => Direction.Left,
                    ConsoleKey.RightArrow => Direction.Right,
                    _ => (Direction?)null
                }).Where(x => x != null).ToList();
                
                directions.ForEach(x => game.ChangeDirection(x!.Value));

                DrawGame(game);
               
                await Task.Delay(20);
            }
        }

        private static bool _drawnOnce = false;
        private static void DrawGame(Game.Game game)
        {
            Console.CursorVisible = false;
            
            DrawGameWalls(game);

            DrawCoins(game);
            
            DrawPacMan(game);

            DrawGhosts(game);
            
            _drawnOnce = true;
        }

        private static void DrawCoins(Game.Game game)
        {
            if (!_drawnOnce)
            {
                foreach (var coin in game.Coins)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;

                    Console.SetCursorPosition(coin.X, coin.Y);
       
                    Console.Write('*');
                }
            }

            var removedCoins = _allCoins.Except(game.Coins)
                .ToList();
            
            foreach (var coin in removedCoins)
            {
                Console.SetCursorPosition(coin.X, coin.Y);
       
                Console.Write(' ');
            }
        }

        private static Dictionary<string, CellLocation> _lastGhostLocation = new Dictionary<string, CellLocation>();

        private static void DrawGhosts(Game.Game game)
        {
            foreach (var ghost in game.Ghosts)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;

                if (_lastGhostLocation.TryGetValue(ghost.Key, out var last))
                {
                    Console.SetCursorPosition(last.X, last.Y);
                    Console.Write(' ');
                }

                Console.SetCursorPosition(ghost.Value.Location.X, ghost.Value.Location.Y);
                var c = GetCharDirection(ghost.Value.Direction);
                Console.Write(c);
                _lastGhostLocation[ghost.Key] = ghost.Value.Location;
            }
        }

        private static void DrawPacMan(Game.Game game)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.SetCursorPosition(_lastPacManLocation.X, _lastPacManLocation.Y);
            Console.Write(' ');

            Console.SetCursorPosition(game.PacMan.Location.X, game.PacMan.Location.Y);
            var c = GetCharDirection(game.PacMan.Direction);
            Console.Write(c);
            _lastPacManLocation = game.PacMan.Location;
        }

        private static char GetCharDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Up => '↑',
                Direction.Down => '↓',
                Direction.Left => '←',
                Direction.Right => '→',
                _ => throw new Exception("Invalid Direction")
            };
        }

        private static void DrawGameWalls(Game.Game game)
        {
            if (_drawnOnce)
                return;
            
            Console.ForegroundColor = ConsoleColor.Blue;
            foreach (var wall in game.Walls)
            {
                Console.SetCursorPosition(wall.X, wall.Y);
                Console.Write("#");
            }
        }

        private static List<ConsoleKey> GetAllKeysPressed()
        {
            var keys = new List<ConsoleKey>();
            while (Console.KeyAvailable)
            {
                keys.Add(Console.ReadKey().Key);
            }

            return keys;
        }
    }
}
