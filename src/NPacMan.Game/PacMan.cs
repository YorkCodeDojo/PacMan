using System;

namespace NPacMan.Game
{
    public class PacMan
    {
        public PacMan(CellLocation location, Direction direction) 
        : this(location, home: location, direction)
        {
        }

        private PacMan(CellLocation location, CellLocation home, Direction direction)
        {
            Location = location;
            Direction = direction;
            Home = home;
        }
        public CellLocation Home { get; }
        public CellLocation Location { get; }
        public Direction Direction { get; }

        public PacMan WithNewX(int newX) => new PacMan(Location.WithNewX(newX), Home, Direction);
        public PacMan WithNewY(int newY) => new PacMan(Location.WithNewY(newY), Home, Direction);
        public PacMan WithNewDirection(Direction newDirection) => new PacMan(Location, Home, newDirection);
        public PacMan SetToHome() => new PacMan(Home, Home, Direction);
        

        internal PacMan Transition(DateTime now)
        {
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
    }
}