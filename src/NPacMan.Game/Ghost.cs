namespace NPacMan.Game
{
    public class Ghost
    {
        public string Name { get; }
        public CellLocation Location { get; }
        public Direction Direction { get; }
        public IGhostStrategy Strategy { get; }
        public IGhostStrategy CurrentStrategy { get; }

        private readonly CellLocation _homeLocation;

        private IGhostStrategy _homeStrategy { get; }

        public int HomeLocationX => _homeLocation.X;
        public int HomeLocationY => _homeLocation.Y;

        public Ghost(string name, CellLocation location, Direction direction, CellLocation homeLocation, IGhostStrategy strategy, IGhostStrategy homeStrategy, IGhostStrategy? currentStrategy = null)
        {
            _homeStrategy = homeStrategy;
            Name = name;
            Location = location;
            Direction = direction;
            Strategy = strategy;
            CurrentStrategy = currentStrategy ?? strategy;
            _homeLocation = homeLocation;
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

            return new Ghost(Name, newLocation, newDirection ?? Direction, _homeLocation, Strategy, _homeStrategy, CurrentStrategy);
        }

        public Ghost GoHome()
        {
            return new Ghost(Name,Location, Direction, _homeLocation, Strategy, _homeStrategy, currentStrategy: _homeStrategy);
        }
    }
}
