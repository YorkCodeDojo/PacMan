using System.Collections.Generic;

namespace NPacMan.Game
{
    public class GameBoard : IGameBoard
    {
        public GameBoard(int width, int height, IReadOnlyCollection<(int,int)> walls, IReadOnlyCollection<(int,int)> coins, IReadOnlyDictionary<(int,int), (int, int)> portals)
        {
            Width = width;
            Height = height;
            Portals = portals;
            Walls = walls;
            Coins = coins;
        }

        public IReadOnlyCollection<(int x, int y)> Walls { get; }
        public IReadOnlyCollection<(int x, int y)> Coins { get; }
        public IReadOnlyDictionary<(int x, int y), (int x, int y)> Portals { get; }
        public int Width { get; }
        public int Height { get; }
        public IReadOnlyCollection<Ghost> Ghosts { get; }
            = new List<Ghost>();
    }
}