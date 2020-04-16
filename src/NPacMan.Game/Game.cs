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
            _timer = new System.Threading.Timer((state) => _action?.Invoke(), null, 0, 500);
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
            PacMan = new PacMan(1, 3, Direction.Down);
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
                (0,0),(1,0),(2,0),(3,0),(4,0),(5,0),(6,0),(7,0),(8,0),(9,0),(10,0),(11,0),(12,0),(13,0),(14,0),(15,0),(16,0),(17,0),(18,0),(19,0),(20,0),(21,0),(22,0),(23,0),(24,0),(25,0),(26,0),
                (0,1),(13,1),(26,1),
                (0,2),  (2,2),(3,2),(4,2),(5,2), (7,2),(8,2),(9,2),(10,2),(11,2),  (13,2),   (15,2),(16,2),(17,2),(18,2),(19,2), (21,2),(22,2),(23,2),(24,2),  (26,2),
                (0,3),  (2,3),(5,3), (7,3),(11,3),  (13,3),   (15,3),(19,3), (21,3),(24,3),  (26,3),
                (0,4),  (2,4),(3,4),(4,4),(5,4), (7,4),(8,4),(9,4),(10,4),(11,4),  (13,4),   (15,4),(16,4),(17,4),(18,4),(19,4), (21,4),(22,4),(23,4),(24,4),  (26,4),
                (0,5),(26,5),
                (0,6),  (2,6),(3,6),(4,6),(5,6), (7,6),(8,6),  (10,6),(11,6),(12,6),(13,6),(14,6),(15,6),(16,6), (18,6),(19,6), (21,6),(22,6),(23,6),(24,6),  (26,6),
                (0,7),  (2,7),(3,7),(4,7),(5,7), (7,7),(8,7),  (10,7),(11,7),(12,7),(13,7),(14,7),(15,7),(16,7), (18,7),(19,7), (21,7),(22,7),(23,7),(24,7),  (26,7),

                (0,8),  (7,8),(8,8),  (13,8), (18,8),(19,8),  (26,8),

                (0,9),(1,9),(2,9),(3,9),(4,9),(5,9), (7,9),(8,9),(9,9),(10,9),(11,9), (13,9), (15,9),(16,9),(17,9),(18,9),(19,9), (21,9),(22,9),(23,9),(24,9),(25,9), (26,9),

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
