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
            var nextDirection = CurrentStrategy.GetNextDirection(this, game);
            if (nextDirection is Direction newDirection)
                return WithNewLocationAndDirection(Location + newDirection, newDirection);
            else
                return this;
        }

        public Ghost Chase()
        {
            return WithNewCurrentStrategyAndDirection(Strategy, Direction.Opposite());
        }

        public Ghost Scatter()
        {
            var strategy = new DirectToStrategy(new DirectToGhostScatterTarget(this));
            return WithNewCurrentStrategyAndDirection(strategy, Direction.Opposite());
        }

        public Ghost SetToHome() => WithNewLocation(Home);

        public Ghost SetToEdible() => WithNewEdible(true);

        internal Ghost SetToNotEdible() => WithNewEdible(false);

        public Ghost WithNewEdible(bool isEdible)
            => new Ghost(Name, Home, Location, Direction, ScatterTarget, Strategy, CurrentStrategy, isEdible);

        public Ghost WithNewCurrentStrategyAndDirection(IGhostStrategy newCurrentStategy, Direction newDirection)
            => new Ghost(Name, Home, Location, newDirection, ScatterTarget, Strategy, newCurrentStategy, Edible);

        public Ghost WithNewLocation(CellLocation newLocation)
            => new Ghost(Name, Home, newLocation, Direction, ScatterTarget, Strategy, CurrentStrategy, Edible);

        public Ghost WithNewLocationAndDirection(CellLocation newLocation, Direction newDirection)
            => new Ghost(Name, Home, newLocation, newDirection, ScatterTarget, Strategy, CurrentStrategy, Edible);

    }
}
