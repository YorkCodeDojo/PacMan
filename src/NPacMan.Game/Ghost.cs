namespace NPacMan.Game
{
    public class Ghost
    {
        public string Name { get; }
        public CellLocation Location { get; }
        public CellLocation HomeLocation { get; }

        public Direction Direction { get; }
        private IGhostStrategy Strategy { get; }
        private IGhostStrategy CurrentStrategy { get; }

        public Ghost(string name, CellLocation location, Direction direction, CellLocation homeLocation, IGhostStrategy strategy) 
        : this(name, location, direction, homeLocation, strategy, strategy)
        {
        }

        private Ghost(string name, CellLocation location, Direction direction, CellLocation homeLocation, IGhostStrategy strategy, IGhostStrategy currentStrategy)
        {
            Name = name;
            Location = location;
            Direction = direction;
            Strategy = strategy;
            CurrentStrategy = currentStrategy;
            HomeLocation = homeLocation;
        }

        public Ghost Move(Game game)
        {
            var newDirection = CurrentStrategy.GetNextDirection(this, game);
            
            var newLocation = newDirection switch 
            {
                Direction.Up => Location.Above,
                Direction.Down => Location.Below,
                Direction.Left => Location.Left,
                Direction.Right => Location.Right,
                _ => Location
            };

            return new Ghost(Name, newLocation, newDirection ?? Direction, HomeLocation, Strategy, CurrentStrategy);
        }

        public Ghost GoHome()
        {
            var homeStrategy = new DirectToStrategy(new DirectToGhostHomeLocation(this));
            
            return new Ghost(Name,Location, Direction, HomeLocation, Strategy, currentStrategy: homeStrategy);
        }
    }
}
