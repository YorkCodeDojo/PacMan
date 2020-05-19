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

        public PacMan WithNewLocation(CellLocation newLocation) => new PacMan(newLocation, Home, Direction);
        
        public PacMan WithNewDirection(Direction newDirection) => new PacMan(Location, Home, newDirection);
        
        public PacMan SetToHome() => new PacMan(Home, Home, Direction);

    }
}