namespace NPacMan.Game
{
    public class Ghost
    {
        public string Name { get; }
        public int X { get; }
        public int Y { get; }
        public IGhostStrategy Strategy { get; }
        public IGhostStrategy CurrentStrategy { get; }
        private IGhostStrategy _homeStrategy { get; }

        public int HomeLocationX => ((MoveHomeGhostStrategy)_homeStrategy).X;
        public int HomeLocationY => ((MoveHomeGhostStrategy)_homeStrategy).Y;

        public Ghost(string name, int x, int y, IGhostStrategy strategy, IGhostStrategy homeStrategy, IGhostStrategy? currentStrategy = null)
        {
            _homeStrategy = homeStrategy;
            Name = name;
            X = x;
            Y = y;
            Strategy = strategy;
            CurrentStrategy = currentStrategy ?? strategy;
        }

        public Ghost Move(Game game)
        {
            var (x, y) = CurrentStrategy.Move(this, game);
            return new Ghost(Name, x, y, Strategy, _homeStrategy, CurrentStrategy);
        }

        public Ghost GoHome()
        {
            return new Ghost(Name, X, Y, Strategy, _homeStrategy, _homeStrategy);
        }
    }
}
