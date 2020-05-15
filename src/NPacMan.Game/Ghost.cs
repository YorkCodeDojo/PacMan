using NPacMan.Game.GhostStrategies;

namespace NPacMan.Game
{
    public class Ghost
    {
        public string Name { get; }
        
        public CellLocation Location { get; }
        public CellLocation ScatterTarget { get; }

        public Direction Direction { get; }

        public CellLocation Home { get; }
        private IGhostStrategy Strategy { get; }
        private IGhostStrategy CurrentStrategy { get; }

        public bool Edible { get; }

        public Ghost(string name, CellLocation location, Direction direction, CellLocation scatterTarget, IGhostStrategy strategy) 
        : this(name, location, location, direction, scatterTarget, strategy, strategy, false)
        {
        }

        private Ghost(string name, CellLocation homeLocation, CellLocation currentLocation, Direction direction, CellLocation scatterTarget, IGhostStrategy strategy, IGhostStrategy currentStrategy, bool edible)
        {
            Name = name;
            Home = homeLocation;
            Location = currentLocation;
            Direction = direction;
            Strategy = strategy;
            CurrentStrategy = currentStrategy;
            ScatterTarget = scatterTarget;
            Edible = edible;
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

            return new Ghost(Name, Home, newLocation, newDirection ?? Direction, ScatterTarget, Strategy, CurrentStrategy, Edible);
        }

        public Ghost Chase()
        {
            var strategy = Strategy;
            
            return new Ghost(Name, Home, Location, Direction.Opposite(), ScatterTarget, Strategy, currentStrategy: strategy, Edible);
        }

        public Ghost Scatter()
        {
            var strategy = new DirectToStrategy(new DirectToGhostScatterTarget(this));
            
            return new Ghost(Name, Home, Location, Direction.Opposite(), ScatterTarget, Strategy, currentStrategy: strategy, Edible);
        }

        public Ghost SetToHome()
        {
            return new Ghost(Name, Home, Home, Direction, ScatterTarget, Strategy, CurrentStrategy, Edible);
        }

        public Ghost SetToEdible()
        {
            return new Ghost(Name, Home, Home, Direction, ScatterTarget, Strategy, CurrentStrategy, edible: true);
        }
    }
}
