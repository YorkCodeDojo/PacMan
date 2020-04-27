using System;

namespace NPacMan.Game
{
    public class PacMan
    {
        public PacMan(int x, int y, Direction direction, PacManStatus newStatus, int lives)
        {
            X = x;
            Y = y;
            Direction = direction;
            Status = newStatus;
            Lives = lives;
        }

        public int X { get; }
        public int Y { get; }
        public int Lives { get; }
        public Direction Direction { get; }
        public PacManStatus Status { get; }

        public PacMan WithNewX(int newX) => new PacMan(newX, Y, Direction, Status, Lives);
        public PacMan WithNewY(int newY) => new PacMan(X, newY, Direction, Status, Lives);
        public PacMan WithNewDirection(Direction newDirection) => new PacMan(X, Y, newDirection, Status, Lives);
        public PacMan WithNewStatus(PacManStatus newStatus) => new PacMan(X, Y, Direction, newStatus, Lives);
        public PacMan WithNewLives(int newLives) => new PacMan(X, Y, Direction, Status, newLives);


        internal PacMan Move()
        {
            switch (Direction)
            {
                case Direction.Up:
                    return WithNewY(Y - 1);

                case Direction.Down:
                    return WithNewY(Y + 1);

                case Direction.Left:
                    return WithNewX(X - 1);

                case Direction.Right:
                    return WithNewX(X + 1);

                default:
                    throw new NotImplementedException();
            }
        }

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        public PacMan Kill() => new PacMan(X, Y, Direction, PacManStatus.Dying, Lives - 1);
    }
}