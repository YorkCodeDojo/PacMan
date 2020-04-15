using System;
using System.Collections.Generic;
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
            _timer = new System.Threading.Timer((state) => _action(), null, 0, 500);
        }
        public void Subscribe(Action action)
        {
            _action = action;
        }
    }
    
    public class Game
    {
        public int Score { get; private set; }

        private IGameBoard _board;
        private List<(int x, int y)> _collectedCoins;

        public Game(IGameClock gameClock, IGameBoard board)
        {
            gameClock.Subscribe(Tick);
            _board = board;
            PacMan = new PacMan(0, 0, Direction.Down);
            _collectedCoins = new List<(int x, int y)>();
        }


        public PacMan PacMan { get; private set; }
        public IReadOnlyCollection<(int x, int y)> Coins
            => _board.Coins.Except(_collectedCoins).ToList().AsReadOnly();
        public IReadOnlyCollection<(int x, int y)> Walls
            => _board.Walls;

        public void ChangeDirection(Direction direction)
        {
            PacMan = new PacMan(PacMan.X, PacMan.Y, direction);
        }

        private void Tick()
        {
            var newPacMan = PacMan.Move();

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

    public interface IGameBoard
    {
        IReadOnlyCollection<(int x, int y)> Walls { get; }
        IReadOnlyCollection<(int x, int y)> Coins { get; }
    }
    public class GameBoard : IGameBoard
    {
        public GameBoard()
        {
            Walls = new[]
            {
                (1,1),
                (1,2),
                (1,3),
                (1,4),
                (0,4),
            };

            Coins = new[]
            {
                (2,1)
            };
        }

        public IReadOnlyCollection<(int x, int y)> Walls { get; }
        public IReadOnlyCollection<(int x, int y)> Coins { get; }
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
