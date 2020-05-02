using System;

namespace NPacMan.Game
{
    public class PacMan
    {
        private readonly DateTime? _timeToChangeState;
        
        public PacMan(int x, int y, Direction direction, PacManStatus newStatus, int lives, DateTime? timeToChangeState = null)
        {
            _timeToChangeState = timeToChangeState;
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

        public PacMan WithNewX(int newX) => new PacMan(newX, Y, Direction, Status, Lives, _timeToChangeState);
        public PacMan WithNewY(int newY) => new PacMan(X, newY, Direction, Status, Lives, _timeToChangeState);
        public PacMan WithNewDirection(Direction newDirection) => new PacMan(X, Y, newDirection, Status, Lives, _timeToChangeState);
        public PacMan WithNewStatus(PacManStatus newStatus) => new PacMan(X, Y, Direction, newStatus, Lives, _timeToChangeState);
        public PacMan WithNewLives(int newLives) => new PacMan(X, Y, Direction, Status, newLives, _timeToChangeState);


        internal PacMan Transition(DateTime now)
        {
            if (Status == PacManStatus.Respawning || Status == PacManStatus.Dead)
            {
                return this;
            }
            if (Status == PacManStatus.Dying)
            {
                if (now >= _timeToChangeState)
                {
                    return WithNewStatus(PacManStatus.Respawning);
                }

                return this;
            }

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

        public PacMan Kill(DateTime timeToDie)
        {
            return new PacMan(X, Y, Direction, PacManStatus.Dying, Lives - 1, timeToDie);
        }
    }
}