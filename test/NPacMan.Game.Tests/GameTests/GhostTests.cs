using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace NPacMan.Game.Tests.GameTests
{
    public class GhostTests
    {
        private readonly TestGameSettings _gameSettings;
        private readonly TestGameClock _gameClock;

        public GhostTests()
        {
            _gameSettings = new TestGameSettings();
            _gameClock = new TestGameClock();
        }

        [Fact]
        public async Task GhostMovesInDirectionOfStrategy()
        {
            var strategy = new GhostGoesRightStrategy();
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", new CellLocation(0, 0), Direction.Left, CellLocation.TopLeft, strategy));

            var game = new Game(_gameClock, _gameSettings);

            await _gameClock.Tick();
            game.Ghosts["Ghost1"].Should().BeEquivalentTo(new
            {
                Location = new {
                    X = 1,
                    Y = 0
                }
            });
        }


        [Fact]
        public async Task GhostShouldNotMoveWhenPacManIsDying()
        {
            var x = 1;
            var y = 1;
            _gameSettings.InitialGameStatus = GameStatus.Dying.ToString();
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", new CellLocation(x, y), Direction.Left, CellLocation.TopLeft, new DirectToStrategy(new DirectToPacManLocation())));
            _gameSettings.PacMan = new PacMan((3, 3), Direction.Down);

            var game = new Game(_gameClock, _gameSettings);
            await _gameClock.Tick();

            using var _ = new AssertionScope();
            game.Ghosts.Values.First()
                .Should().BeEquivalentTo(new
                {
                    Location = new {
                        X = x,
                        Y = y
                    }
                });
        }



        [Fact]
        public async Task GhostShouldBeHiddenWhenPacManIsReSpawning()
        {
            _gameSettings.PacMan = new PacMan((3, 2), Direction.Left);
            var homeLocation = new CellLocation(1, 2);
            var strategy = new GhostGoesRightStrategy();
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", homeLocation, Direction.Right, CellLocation.TopLeft, strategy));

            var game = new Game(_gameClock, _gameSettings);
            var now = DateTime.UtcNow;

            await _gameClock.Tick(now);
            await _gameClock.Tick(now.AddSeconds(4));

            game.Ghosts.Should().BeEmpty();
        }

        [Fact]
        public async Task GhostShouldBeBackAtHomeAfterPacManDiesAndComesBackToLife()
        {
            _gameSettings.PacMan = new PacMan((3, 2), Direction.Left);
            var homeLocation = new CellLocation(1, 2);
            var strategy = new GhostGoesRightStrategy();
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", homeLocation, Direction.Right, CellLocation.TopLeft, strategy));

            var game = new Game(_gameClock, _gameSettings);
            var now = DateTime.UtcNow;

            await _gameClock.Tick(now);
            await _gameClock.Tick(now.AddSeconds(4));

            if (game.Status != GameStatus.Respawning.ToString())
                throw new Exception($"Invalid Game State {game.Status:G} Should be Respawning");

            await _gameClock.Tick(now.AddSeconds(8));
            
            if (game.Status != GameStatus.Alive.ToString())
                throw new Exception($"Invalid Game State {game.Status:G} Should be Alive");
            
            game.Ghosts.Values.First()
                .Should().BeEquivalentTo(new
                {
                    Location = new {
                        X = homeLocation.X,
                        Y = homeLocation.Y
                    }
                });
        }

        [Fact]
        public async Task GhostShouldScatterToStartWith()
        {
            _gameSettings.InitialGameStatus = "Initial";
            _gameSettings.PacMan = new PacMan((10, 10), Direction.Left);
            var startingLocation = new CellLocation(3, 1);
            var scatterLocation = new CellLocation(1, 1);

            var strategy = new GhostGoesRightStrategy();
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", startingLocation, Direction.Right, scatterLocation, strategy));

            var game = new Game(_gameClock, _gameSettings);

            await _gameClock.Tick();
            await _gameClock.Tick();
            await _gameClock.Tick();

            game.Ghosts.Values.First()
                .Should().BeEquivalentTo(new
                {
                    Location = new {
                        X = scatterLocation.X,
                        Y = scatterLocation.Y
                    }
                });
        }

        [Fact]
        public async Task GhostShouldChaseAfter7Seconds()
        {
            _gameSettings.InitialGameStatus = "Initial";
            _gameSettings.PacMan = new PacMan((29, 10), Direction.Left);
            var startingLocation = new CellLocation(30, 1);
            var scatterLocation = new CellLocation(1, 1);

            var strategy = new GhostGoesRightStrategy();
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", startingLocation, Direction.Right, scatterLocation, strategy));

            var game = new Game(_gameClock, _gameSettings);

            var now = DateTime.UtcNow;
            await _gameClock.Tick(now);
            await _gameClock.Tick(now);

            if (game.Ghosts.Values.First().Location.X != 29 || game.Ghosts.Values.First().Location.Y != 1)
                throw new System.Exception($"Ghost should be at 29,1 not {game.Ghosts.Values.First().Location.X}, {game.Ghosts.Values.First().Location.Y}");

            await _gameClock.Tick(now.AddSeconds(_gameSettings.InitialScatterTimeInSeconds + 1));
            await _gameClock.Tick(now.AddSeconds(_gameSettings.InitialScatterTimeInSeconds + 2));

            game.Ghosts.Values.First()
                .Should().BeEquivalentTo(new
                {
                    Location = new {
                        X = 31,
                        Y = scatterLocation.Y
                    }
                });
        }

        [Fact]
        public async Task GhostShouldScatter7SecondsAfterChase()
        {
            _gameSettings.InitialGameStatus = "Initial";
            _gameSettings.PacMan = new PacMan((29, 10), Direction.Left);
            var startingLocation = new CellLocation(30, 1);
            var scatterLocation = new CellLocation(1, 1);

            var strategy = new GhostGoesRightStrategy();
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", startingLocation, Direction.Right, scatterLocation, strategy));

            var game = new Game(_gameClock, _gameSettings);

            var now = DateTime.UtcNow;
            await _gameClock.Tick(now);
            await _gameClock.Tick(now);

            if (game.Ghosts.Values.First().Location.X != 29 || game.Ghosts.Values.First().Location.Y != 1)
                throw new System.Exception($"Ghost should be at 29,1 not {game.Ghosts.Values.First().Location.X}, {game.Ghosts.Values.First().Location.Y}");

            await _gameClock.Tick(now.AddSeconds(_gameSettings.InitialScatterTimeInSeconds + 1));
            await _gameClock.Tick(now.AddSeconds(_gameSettings.InitialScatterTimeInSeconds + 2));

            if (game.Ghosts.Values.First().Location.X != 31 || game.Ghosts.Values.First().Location.Y != 1)
                throw new System.Exception($"Ghost should be at 31,1 not {game.Ghosts.Values.First().Location.X}, {game.Ghosts.Values.First().Location.Y}");

            now = now.AddSeconds(_gameSettings.InitialScatterTimeInSeconds);

            await _gameClock.Tick(now.AddSeconds(_gameSettings.ChaseTimeInSeconds + 1));
            await _gameClock.Tick(now.AddSeconds(_gameSettings.ChaseTimeInSeconds + 2));

            game.Ghosts.Values.First()
                .Should().BeEquivalentTo(new
                {
                    Location = new {
                        X = 29,
                        Y = 1
                    }
                });
        }

    }
}
