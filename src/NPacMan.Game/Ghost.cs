namespace NPacMan.Game
{
    public class Ghost
    {
        public string Name { get; }
        public int X { get; }
        public int Y { get; }
        public IGhostStrategy Strategy { get; }
        public IGhostStrategy CurrentStrategy { get; }

        private readonly CellLocation _homeLocation;

        private IGhostStrategy _homeStrategy { get; }

        public int HomeLocationX => _homeLocation.X;
        public int HomeLocationY => _homeLocation.Y;

        public Ghost(string name, int x, int y, CellLocation homeLocation, IGhostStrategy strategy, IGhostStrategy homeStrategy, IGhostStrategy? currentStrategy = null)
        {
            _homeStrategy = homeStrategy;
            Name = name;
            X = x;
            Y = y;
            Strategy = strategy;
            CurrentStrategy = currentStrategy ?? strategy;
            _homeLocation = homeLocation;
        }

        public Ghost Move(Game game)
        {
            var (x, y) = CurrentStrategy.Move(this, game);
            return new Ghost(Name, x, y, _homeLocation, Strategy, _homeStrategy, CurrentStrategy);
        }

        public Ghost GoHome()
        {
            return new Ghost(Name, X, Y, _homeLocation, Strategy, _homeStrategy, _homeStrategy);
        }
    }
}
