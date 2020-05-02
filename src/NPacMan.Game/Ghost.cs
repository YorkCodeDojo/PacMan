namespace NPacMan.Game
{
    public class Ghost
    {
        public string Name { get; }
        public int X { get; }
        public int Y { get; }
        public IGhostStrategy Strategy { get; }
        public IGhostStrategy CurrentStrategy { get; }
        public IGhostStrategy HomeStrategy { get; }

        public Ghost(string name, int x, int y, IGhostStrategy strategy, IGhostStrategy homeStrategy, IGhostStrategy? currentStrategy = null)
        {
            HomeStrategy = homeStrategy;
            Name = name;
            X = x;
            Y = y;
            Strategy = strategy;
            CurrentStrategy = currentStrategy ?? strategy;
        }

        public Ghost Move(Game game)
        {
            var (x, y) = CurrentStrategy.Move(this, game);
            return new Ghost(Name, x, y, Strategy, HomeStrategy, CurrentStrategy);
        }

        public Ghost GoHome()
        {
            return new Ghost(Name, X, Y, Strategy, HomeStrategy, HomeStrategy);
        }
    }
}