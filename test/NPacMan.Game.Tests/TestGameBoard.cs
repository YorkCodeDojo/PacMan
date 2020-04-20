using System.Collections.Generic;

namespace NPacMan.Game.Tests
{
    public class TestGameBoard : IGameBoard
    {
        public List<(int x, int y)> Walls { get; set; }
            = new List<(int x, int y)>();

        IReadOnlyCollection<(int x, int y)> IGameBoard.Walls
            => this.Walls;

        public List<(int x, int y)> Coins { get; set; }
            = new List<(int x, int y)>();

        IReadOnlyCollection<(int x, int y)> IGameBoard.Coins
            => this.Coins;

        public Dictionary<(int x, int y), (int x, int y)> Portals { get; set; }
            = new Dictionary<(int x, int y), (int x, int y)>();

        IReadOnlyDictionary<(int x, int y), (int x, int y)> IGameBoard.Portals
            => this.Portals;

        public int Width { get; set; }

        public int Height { get; set; }
    }
}