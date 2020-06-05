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
            IReadOnlyCollection<CellLocation> doors,
            IReadOnlyCollection<CellLocation> ghostHouse,
            CellLocation fruit)
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
            GhostHouse = ghostHouse;
            Fruit = fruit;
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
        public IReadOnlyCollection<CellLocation> GhostHouse { get; }
        public CellLocation Fruit { get; }
        public GameStatus InitialGameStatus { get; } = GameStatus.Initial;
        public int InitialLives { get; } = 3;

        public int InitialScatterTimeInSeconds { get; } = 7;

        public int ChaseTimeInSeconds { get; } = 7;

        public int FrightenedTimeInSeconds { get; } = 7;

        public IDirectionPicker DirectionPicker {get;} = new RandomDirectionPicker();

        public IReadOnlyCollection<int> FruitAppearsAfterCoinsEaten {get;}= new []{
            70, 170
        };

        public int FruitVisibleForSeconds { get; } = 9;
    }
}