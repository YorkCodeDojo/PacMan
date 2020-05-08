using System;

namespace NPacMan.Game
{
    public class PacMan
    {
        private readonly DateTime? _timeToChangeState;
        
        public PacMan(int x, int y, Direction direction, PacManStatus newStatus, int lives, DateTime? timeToChangeState = null) 
        : this(x,y, new CellLocation(x,y), direction, newStatus, lives, timeToChangeState)
        {
        }

        private PacMan(int x, int y, CellLocation home, Direction direction, PacManStatus newStatus, int lives, DateTime? timeToChangeState = null)
        {
            _timeToChangeState = timeToChangeState;
            X = x;
            Y = y;
            Direction = direction;
            Status = newStatus;
            Lives = lives;
            Home = home;
        }


        public int X { get; }
        public int Y { get; }
        public int Lives { get; }
        public CellLocation Home { get; }
        public Direction Direction { get; }
        public PacManStatus Status { get; }

        public PacMan WithNewX(int newX) => new PacMan(newX, Y, Home, Direction, Status, Lives, _timeToChangeState);
        public PacMan WithNewY(int newY) => new PacMan(X, newY, Home, Direction, Status, Lives, _timeToChangeState);
        public PacMan WithNewDirection(Direction newDirection) => new PacMan(X, Y, Home, newDirection, Status, Lives, _timeToChangeState);
        public PacMan WithNewStatus(PacManStatus newStatus) => new PacMan(X, Y, Home, Direction, newStatus, Lives, _timeToChangeState);
        public PacMan WithNewLives(int newLives) => new PacMan(X, Y, Home, Direction, Status, newLives, _timeToChangeState);


        internal PacMan Transition(DateTime now)
        {
            if (Status == PacManStatus.Dead)
            {
                return this;
            }
            
            if (Status == PacManStatus.Dying)
            {
                if (now >= _timeToChangeState)
                {
                    return Respawn(now.AddSeconds(4));
                }

                return this;
            }

            if (Status == PacManStatus.Respawning)
            {
                if (now >= _timeToChangeState)
                {
                    return Resurrect();
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
            return new PacMan(X, Y, Home, Direction, PacManStatus.Dying, Lives - 1, timeToDie);
        }

        public PacMan Respawn(DateTime timeToBecomeAlive)
        {
            return new PacMan(X, Y, Home, Direction, PacManStatus.Respawning, Lives, timeToBecomeAlive);
        }

        public PacMan Resurrect()
        {
            return new PacMan(Home.X, Home.Y, Home, Direction, PacManStatus.Alive, Lives, null);
        }

        
    }
}