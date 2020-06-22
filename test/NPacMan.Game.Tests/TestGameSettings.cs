using System;
using System.Collections.Generic;
using NPacMan.Game.Tests.GameTests;

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

        public List<CellLocation> PowerPills { get; set; }
            = new List<CellLocation>();

        IReadOnlyCollection<CellLocation> IGameSettings.PowerPills
            => this.PowerPills;
            
        public List<CellLocation> Doors { get; set; }
            = new List<CellLocation>();

        IReadOnlyCollection<CellLocation> IGameSettings.Doors
            => this.Doors;

        public Dictionary<CellLocation, CellLocation> Portals { get; set; }
            = new Dictionary<CellLocation, CellLocation>();

        IReadOnlyDictionary<CellLocation, CellLocation> IGameSettings.Portals
            => this.Portals;


        public List<CellLocation> GhostHouse { get; set; }
            = new List<CellLocation>(){new CellLocation(100, 100)};
            
        IReadOnlyCollection<CellLocation> IGameSettings.GhostHouse
            => this.GhostHouse;


        public int Width { get; set; }

        public int Height { get; set; }

        public List<Ghost> Ghosts { get; }
            = new List<Ghost>();

        IReadOnlyCollection<Ghost> IGameSettings.Ghosts
            => this.Ghosts;

        public List<int> FruitAppearsAfterCoinsEaten { get; }
            = new List<int>();
        IReadOnlyCollection<int> IGameSettings.FruitAppearsAfterCoinsEaten
            => this.FruitAppearsAfterCoinsEaten;


        public CellLocation Fruit { get; set; }

        public PacMan PacMan { get; set; } = new PacMan(new CellLocation(10, 10), Direction.Right);
        public GameStatus InitialGameStatus { get; set; } = GameStatus.Alive;
        public int InitialLives { get; set; } = 3;
        public int InitialScatterTimeInSeconds { get; set; } = 7;
        public int ChaseTimeInSeconds { get; set; } = 7;
        public int FrightenedTimeInSeconds { get; set; } = 7;
        public IDirectionPicker DirectionPicker { get; internal set; } = new TestDirectionPicker();
        public int FruitVisibleForSeconds { get; set; } = 7;
        public int PointsNeededForBonusLife { get; internal set; } = int.MaxValue;
    }
}