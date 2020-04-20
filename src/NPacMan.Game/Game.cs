using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace NPacMan.Game
{
    public class GameClock : IGameClock
    {
        private Action _action;
        private Timer _timer;

        public GameClock()
        {
            _timer = new System.Threading.Timer((state) => _action?.Invoke(), null, 0, 200);
        }
        public void Subscribe(Action action)
        {
            _action = action;
        }
    }

    public class Game
    {
        public int Score { get; private set; }

        private readonly IGameBoard _board;
        private readonly List<(int x, int y)> _collectedCoins;

        public Game(IGameClock gameClock, IGameBoard board)
        {
            gameClock.Subscribe(Tick);
            _board = board;
            PacMan = new PacMan(1, 3, Direction.Down);
            _collectedCoins = new List<(int x, int y)>();
        }

        public static Game Create()
        {
            var board = @" XXXXXXXXXXXXXXXXXXXXXXXXXXXX
 X............XX............X
 X.XXXX.XXXXX.XX.XXXXX.XXXX.X
 X.X  X.X   X.XX.X   X.X  X.X
 X.XXXX.XXXXX.XX.XXXXX.XXXX.X
 X..........................X
 X.XXXX.XX.XXXXXXXX.XX.XXXX.X
 X.XXXX.XX.XXXXXXXX.XX.XXXX.X
 X......XX....XX....XX......X
 XXXXXX.XXXXX XX XXXXX.XXXXXX
      X.XXXXX XX XXXXX.X     
      X.XX          XX.X     
      X.XX          XX.X     
 XXXXXX.XX  XXXXXX  XX.XXXXXX
P      .    X    X    .      P
 XXXXXX.XX  XXXXXX  XX.XXXXXX
      X.XX          XX.X     
      X.XX          XX.X     
      X.XX XXXXXXXX XX.X
 XXXXXX.XX XXXXXXXX XX.XXXXXX
 X............XX............X
 X.XXXX.XXXXX.XX.XXXXX.XXXX.X
 X.XXXX.XXXXX.XX.XXXXX.XXXX.X
 X...XX................XX...X
 XXX.XX.XX.XXXXXXXX.XX.XX.XXX
 XXX.XX.XX.XXXXXXXX.XX.XX.XXX
 X......XX....XX....XX......X
 X.XXXXXXXXXX.XX.XXXXXXXXXX.X
 X.XXXXXXXXXX.XX.XXXXXXXXXX.X
 X..........................X
 XXXXXXXXXXXXXXXXXXXXXXXXXXXX
";

            return new Game(new GameClock(), BoardLoader.Load(board));
        }


        public PacMan PacMan { get; private set; }
        public IReadOnlyCollection<(int x, int y)> Coins
            => _board.Coins.Except(_collectedCoins).ToList().AsReadOnly();
        public IReadOnlyCollection<(int x, int y)> Walls
            => _board.Walls;

        public int Width
            => _board.Width;

        public void ChangeDirection(Direction direction)
        {
            PacMan = new PacMan(PacMan.X, PacMan.Y, direction);
        }

        private void Tick()
        {
            var newPacMan = PacMan.Move();

            if (_board.Portals.TryGetValue((newPacMan.X, newPacMan.Y), out var portal))
            {
                newPacMan = new PacMan(portal.x, portal.y, newPacMan.Direction);
                newPacMan = newPacMan.Move();
            }

            if (!_board.Walls.Contains((newPacMan.X, newPacMan.Y)))
            {
                PacMan = newPacMan;

                if (_board.Coins.Contains((newPacMan.X, newPacMan.Y)))
                {
                    _collectedCoins.Add((newPacMan.X, newPacMan.Y));
                    Score += 10;
                }
            }
        }
    }
    
    public class BoardLoader
    {
        public static IGameBoard Load(string board)
        {
            var rows = board.Split(new []{Environment.NewLine}, StringSplitOptions.None);
            var height = rows.Length;
            var width = 0;

            var coins = new List<(int, int)>();
            var walls = new List<(int, int)>();
            var portalParts = new List<(int, int)>();

            for (int rowNumber = 0; rowNumber < height; rowNumber++)
            {
                var row = rows[rowNumber];
                for (int columnNumber = 0; columnNumber < row.Length; columnNumber++)
                {
                    switch (row[columnNumber])
                    {
                        case 'X':
                            walls.Add((columnNumber-1, rowNumber));
                            break;
                        case '.':
                            coins.Add((columnNumber-1,rowNumber));
                            break;
                        case 'P':
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

            return new GameBoard(width-2, height, walls, coins, portals);
        }
    }

    public interface IGameBoard
    {
        IReadOnlyCollection<(int x, int y)> Walls { get; }
        IReadOnlyCollection<(int x, int y)> Coins { get; }
        IReadOnlyDictionary<(int x, int y), (int x, int y)> Portals { get; }
        int Width { get; }
        int Height { get; }
    }

    public class GameBoard : IGameBoard
    {
        public GameBoard(int width, int height, IReadOnlyCollection<(int,int)> walls, IReadOnlyCollection<(int,int)> coins, IReadOnlyDictionary<(int,int), (int, int)> portals)
        {
            Width = width;
            Height = height;
            Portals = portals;
            Walls = walls;
            Coins = coins;
        }

        public IReadOnlyCollection<(int x, int y)> Walls { get; }
        public IReadOnlyCollection<(int x, int y)> Coins { get; }
        public IReadOnlyDictionary<(int x, int y), (int x, int y)> Portals { get; }
        public int Width { get; }
        public int Height { get; }
    }

    public class PacMan
    {
        internal PacMan(int x, int y, Direction direction)
        {
            X = x;
            Y = y;
            Direction = direction;
        }

        public int X { get; }
        public int Y { get; }
        public Direction Direction { get; }

        public PacMan WithNewX(int newX) => new PacMan(newX, Y, Direction);
        public PacMan WithNewY(int newY) => new PacMan(X, newY, Direction);
        public PacMan WithNewDirection(Direction newDirection) => new PacMan(X, Y, newDirection);

        internal PacMan Move()
        {
            switch (Direction)
            {
                case Direction.Up:
                    return new PacMan(X, Y - 1, Direction);

                case Direction.Down:
                    return new PacMan(X, Y + 1, Direction);

                case Direction.Left:
                    return new PacMan(X - 1, Y, Direction);

                case Direction.Right:
                    return new PacMan(X + 1, Y, Direction);

                default:
                    throw new NotImplementedException();
            }
        }

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }
    }
}
