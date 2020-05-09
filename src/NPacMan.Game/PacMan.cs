using System;

namespace NPacMan.Game
{
    public class PacMan
    {
        private readonly DateTime? _timeToChangeState;
        
        public PacMan(CellLocation location, Direction direction, PacManStatus newStatus, int lives, DateTime? timeToChangeState = null) 
        : this(location, home: location, direction, newStatus, lives, timeToChangeState)
        {
        }

        private PacMan(CellLocation location, CellLocation home, Direction direction, PacManStatus newStatus, int lives, DateTime? timeToChangeState = null)
        {
            _timeToChangeState = timeToChangeState;
            Location = location;
            Direction = direction;
            Status = newStatus;
            Lives = lives;
            Home = home;
        }
        public int Lives { get; }
        public CellLocation Home { get; }
        public CellLocation Location { get; }
        public Direction Direction { get; }
        public PacManStatus Status { get; }

        public PacMan WithNewX(int newX) => new PacMan(Location.WithNewX(newX), Home, Direction, Status, Lives, _timeToChangeState);
        public PacMan WithNewY(int newY) => new PacMan(Location.WithNewY(newY), Home, Direction, Status, Lives, _timeToChangeState);
        public PacMan WithNewDirection(Direction newDirection) => new PacMan(Location, Home, newDirection, Status, Lives, _timeToChangeState);
        public PacMan WithNewStatus(PacManStatus newStatus) => new PacMan(Location, Home, Direction, newStatus, Lives, _timeToChangeState);
        public PacMan WithNewLives(int newLives) => new PacMan(Location, Home, Direction, Status, newLives, _timeToChangeState);


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
                    return WithNewY(Location.Y - 1);

                case Direction.Down:
                    return WithNewY(Location.Y + 1);

                case Direction.Left:
                    return WithNewX(Location.X - 1);

                case Direction.Right:
                    return WithNewX(Location.X + 1);

                default:
                    throw new NotImplementedException();
            }
        }

        public PacMan Kill(DateTime timeToDie)
        {
            return new PacMan(Location, Home, Direction, PacManStatus.Dying, Lives - 1, timeToDie);
        }

        public PacMan Respawn(DateTime timeToBecomeAlive)
        {
            return new PacMan(Location, Home, Direction, PacManStatus.Respawning, Lives, timeToBecomeAlive);
        }

        public PacMan Resurrect()
        {
            return new PacMan(Home, Home, Direction, PacManStatus.Alive, Lives, null);
        }

        
    }
}