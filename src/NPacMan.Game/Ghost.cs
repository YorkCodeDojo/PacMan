namespace NPacMan.Game
{
    public class Ghost
    {
        public string Name { get; }
        public CellLocation Location { get; }
        public CellLocation ScatterTarget { get; }

        public Direction Direction { get; }
        private IGhostStrategy Strategy { get; }
        private IGhostStrategy CurrentStrategy { get; }

        public Ghost(string name, CellLocation location, Direction direction, CellLocation scatterTarget, IGhostStrategy strategy) 
        : this(name, location, direction, scatterTarget, strategy, strategy)
        {
        }

        private Ghost(string name, CellLocation location, Direction direction, CellLocation scatterTarget, IGhostStrategy strategy, IGhostStrategy currentStrategy)
        {
            Name = name;
            Location = location;
            Direction = direction;
            Strategy = strategy;
            CurrentStrategy = currentStrategy;
            ScatterTarget = scatterTarget;
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

            return new Ghost(Name, newLocation, newDirection ?? Direction, ScatterTarget, Strategy, CurrentStrategy);
        }

        public Ghost Scatter()
        {
            var strategy = new DirectToStrategy(new DirectToGhostScatterTarget(this));
            
            return new Ghost(Name,Location, Direction, ScatterTarget, Strategy, currentStrategy: strategy);
        }
    }
}
