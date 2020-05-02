namespace NPacMan.Game
{
    public class Ghost
    {
        private readonly IGhostStrategy _homeStrategy;
        public string Name { get; }
        public int X { get; }
        public int Y { get; }
        public IGhostStrategy Strategy { get; }

        public Ghost(string name, int x, int y, IGhostStrategy strategy, IGhostStrategy homeStrategy)
        {
            _homeStrategy = homeStrategy;
            Name = name;
            X = x;
            Y = y;
            Strategy = strategy;
        }

        public Ghost Move(Game game)
        {
            var (x, y) = Strategy.Move(this, game);
            return new Ghost(Name, x, y, Strategy, _homeStrategy);
        }

        public Ghost GoHome()
        {
            return new Ghost(Name, X, Y, _homeStrategy, Strategy);
        }
    }
}