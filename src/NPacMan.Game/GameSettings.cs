using System.Collections.Generic;

namespace NPacMan.Game
{
    public class GameSettings : IGameSettings
    {
        public GameSettings(
            int width,
            int height,
            IReadOnlyCollection<CellLocation> walls,
            IReadOnlyCollection<CellLocation> coins,
            IReadOnlyCollection<CellLocation> powerPills,
            IReadOnlyDictionary<CellLocation, CellLocation> portals,
            PacMan pacMan,
            IReadOnlyCollection<Ghost> ghosts,
            IReadOnlyCollection<CellLocation> doors)
        {
            Width = width;
            Height = height;
            Portals = portals;
            PacMan = pacMan;
            Ghosts = ghosts;
            Walls = walls;
            Coins = coins;
            PowerPills = powerPills;
            Doors = doors;
        }

        public PacMan PacMan { get; }
        public IReadOnlyCollection<CellLocation> Walls { get; }
        public IReadOnlyCollection<CellLocation> Coins { get; }
        public IReadOnlyCollection<CellLocation> PowerPills { get; }
        public IReadOnlyDictionary<CellLocation, CellLocation> Portals { get; }
        public int Width { get; }
        public int Height { get; }
        public IReadOnlyCollection<Ghost> Ghosts { get; }
        public IReadOnlyCollection<CellLocation> Doors { get; }
        public GameStatus InitialGameStatus { get; } = GameStatus.Initial;
        public int InitialLives { get; } = 3;

        public int InitialScatterTimeInSeconds { get; } = 7;

        public int ChaseTimeInSeconds { get; } = 7;

    }
}