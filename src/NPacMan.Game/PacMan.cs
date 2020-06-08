using System;

namespace NPacMan.Game
{
    public class PacMan
    {
        public PacMan(CellLocation location, Direction direction)
        {
            Location = location;
            Direction = direction;
        }
        public CellLocation Location { get; }
        public Direction Direction { get; }

        public PacMan WithNewLocation(CellLocation newLocation) => new PacMan(newLocation, Direction);
        
        public PacMan WithNewDirection(Direction newDirection) => new PacMan(Location, newDirection);

    }
}