using System.Collections.Generic;

namespace NPacMan.Game
{
    public interface IGhostStrategy
    {
        (int x, int y) Move(Ghost ghost);
    }

    public interface IGameBoard
    {
        IReadOnlyCollection<(int x, int y)> Walls { get; }
        IReadOnlyCollection<(int x, int y)> Coins { get; }
        IReadOnlyDictionary<(int x, int y), (int x, int y)> Portals { get; }
        int Width { get; }
        int Height { get; }
        IReadOnlyCollection<Ghost> Ghosts { get; }
    }

    public class Ghost
    {
        public string Name { get; }
        public int X { get; }
        public int Y { get; }
        public IGhostStrategy Strategy { get; }

        public Ghost(string name, int x, int y, IGhostStrategy strategy)
        {
            Name = name;
            X = x;
            Y = y;
            Strategy = strategy;
        }

        public Ghost Move()
        {
            var (x, y) = Strategy.Move(this);
            return new Ghost(Name, x, y, Strategy);
        }
    }
}