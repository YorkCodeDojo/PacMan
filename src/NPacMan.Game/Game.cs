using System;
using System.Collections.Generic;
using System.Linq;

namespace NPacMan.Game
{
    public class Game
    {
        private IGameBoard _board;

        public Game(IGameClock gameClock, IGameBoard board)
        {
            gameClock.Subscribe(Tick);
            _board = board;
            PacMan = new PacMan(10, 10, Direction.Down);
        }


        public PacMan PacMan { get; private set; }

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
            }
        }
    }

    public interface IGameBoard
    {
        IReadOnlyCollection<(int x, int y)> Walls { get; }
    }
    public class Board : IGameBoard
    {
        public Board()
        {
            Walls = new[]
            {
                (1,2),
                (1,2),
            };
        }

        public IReadOnlyCollection<(int x, int y)> Walls { get; }
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
    }
}
