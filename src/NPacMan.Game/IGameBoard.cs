using System.Collections.Generic;

namespace NPacMan.Game
{
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
        public int X { get; }
        public int Y { get; }

        public Ghost(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}