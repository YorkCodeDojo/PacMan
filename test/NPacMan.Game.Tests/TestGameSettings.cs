using System.Collections.Generic;

namespace NPacMan.Game.Tests
{
    public class TestGameSettings : IGameSettings
    {
        public List<CellLocation> Walls { get; set; }
            = new List<CellLocation>();

        IReadOnlyCollection<CellLocation> IGameSettings.Walls
            => this.Walls;

        public List<CellLocation> Coins { get; set; }
            = new List<CellLocation>();

        IReadOnlyCollection<CellLocation> IGameSettings.Coins
            => this.Coins;

        public List<CellLocation> Doors { get; set; }
            = new List<CellLocation>();

        IReadOnlyCollection<CellLocation> IGameSettings.Doors
            => this.Doors;

        public Dictionary<CellLocation, CellLocation> Portals { get; set; }
            = new Dictionary<CellLocation, CellLocation>();

        IReadOnlyDictionary<CellLocation, CellLocation> IGameSettings.Portals
            => this.Portals;

        public int Width { get; set; }

        public int Height { get; set; }

        public List<Ghost> Ghosts { get; }
            = new List<Ghost>();

        IReadOnlyCollection<Ghost> IGameSettings.Ghosts
            => this.Ghosts;

        public PacMan PacMan { get; set; } = new PacMan(new CellLocation(10, 10), Direction.Right);
        public string InitialGameStatus { get; set; } = "Alive";
        public int InitialLives { get; set; } = 3;
        public int InitialScatterTimeInSeconds { get; set; } = 7;
        public int ChaseTimeInSeconds { get; set; } = 7;
    }
}