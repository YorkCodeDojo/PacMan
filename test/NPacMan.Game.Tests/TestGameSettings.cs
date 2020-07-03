using NPacMan.Game.Tests.GameTests;
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
            = new List<CellLocation>() { new CellLocation(100, 100) };

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
        public GameStatus InitialGameStatus { get; set; } = GameStatus.Initial;
        public int InitialLives { get; set; } = 3;
        public int InitialScatterTimeInSeconds { get; set; } = 7;
        public int ChaseTimeInSeconds { get; set; } = 7;
        public int FruitVisibleForSeconds { get; set; } = 7;
        public int PointsNeededForBonusLife { get; internal set; } = int.MaxValue;
        public IHighScoreStorage HighScoreStorage { get; internal set; } = new InMemoryHighScoreStorage();
        public int FrightenedFlashTimeInSeconds { get; internal set; } = 2;

        public IMoveClock MoveClock { get; internal set; } = new TestMoveClock();


        public List<CellLocation> Tunnels { get; set; }
            = new List<CellLocation>();

        IReadOnlyCollection<CellLocation> IGameSettings.Tunnels
            => this.Tunnels;

        private readonly AsciiTestSettingBuilder _builder;

        public TestGameSettings()
        {
            _builder = new AsciiTestSettingBuilder(this);
        }

        internal void CreateBoard(params string[] rows)
        {
            _builder.CreateBoard(rows);
        }

        internal void AssertBoard(Game game, params string[] rows)
        {
            _builder.AssertBoard(game, rows);
        }
    }
}