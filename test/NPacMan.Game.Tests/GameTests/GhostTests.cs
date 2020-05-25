using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using NPacMan.Game.GhostStrategies;
using NPacMan.Game.Tests.GhostStrategiesForTests;
using NPacMan.Game.Tests.Helpers;
using Xunit;
using static NPacMan.Game.Tests.GameTests.Ensure;

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
            game.StartGame();
            await _gameClock.Tick();
            game.Ghosts["Ghost1"].Should().BeEquivalentTo(new
            {
                Location = new
                {
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
            _gameSettings.InitialGameStatus = GameStatus.Dying;
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", new CellLocation(x, y), Direction.Left, CellLocation.TopLeft, new DirectToStrategy(new DirectToPacManLocation())));
            _gameSettings.PacMan = new PacMan((3, 3), Direction.Down);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();
            await _gameClock.Tick();

            using var _ = new AssertionScope();
            game.Ghosts.Values.First()
                .Should().BeEquivalentTo(new
                {
                    Location = new
                    {
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
            game.StartGame();
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
            game.StartGame();
            var now = DateTime.UtcNow;

            await _gameClock.Tick(now);
            await _gameClock.Tick(now.AddSeconds(4));

            if (game.Status != GameStatus.Respawning)
                throw new Exception($"Invalid Game State {game.Status:G} Should be Respawning");

            await _gameClock.Tick(now.AddSeconds(8));

            if (game.Status != GameStatus.Alive)
                throw new Exception($"Invalid Game State {game.Status:G} Should be Alive");

            game.Ghosts.Values.First()
                .Should().BeEquivalentTo(new
                {
                    Location = new
                    {
                        X = homeLocation.X,
                        Y = homeLocation.Y
                    }
                });
        }

        [Fact]
        public async Task GhostShouldScatterToStartWith()
        {
            _gameSettings.InitialGameStatus = GameStatus.Initial;
            _gameSettings.PacMan = new PacMan((10, 10), Direction.Left);
            var startingLocation = new CellLocation(3, 1);
            var scatterLocation = new CellLocation(1, 1);

            var strategy = new GhostGoesRightStrategy();
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", startingLocation, Direction.Right, scatterLocation, strategy));

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();
            await _gameClock.Tick();
            await _gameClock.Tick();
            await _gameClock.Tick();

            game.Ghosts.Values.First()
                .Should().BeEquivalentTo(new
                {
                    Location = new
                    {
                        X = scatterLocation.X,
                        Y = scatterLocation.Y
                    }
                });
        }

        [Fact]
        public async Task GhostShouldChaseAfter7Seconds()
        {
            _gameSettings.InitialGameStatus = GameStatus.Initial;
            _gameSettings.PacMan = new PacMan((29, 10), Direction.Left);
            var startingLocation = new CellLocation(30, 1);
            var scatterLocation = new CellLocation(1, 1);

            var strategy = new GhostGoesRightStrategy();
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", startingLocation, Direction.Right, scatterLocation, strategy));

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();
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
                    Location = new
                    {
                        X = 31,
                        Y = scatterLocation.Y
                    }
                });
        }

        [Fact]
        public async Task GhostShouldScatter7SecondsAfterChase()
        {
            _gameSettings.InitialGameStatus = GameStatus.Initial;
            _gameSettings.PacMan = new PacMan((29, 10), Direction.Left);
            var startingLocation = new CellLocation(30, 1);
            var scatterLocation = new CellLocation(1, 1);

            var strategy = new GhostGoesRightStrategy();
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", startingLocation, Direction.Right, scatterLocation, strategy));

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();
            var now = DateTime.UtcNow;

            await _gameClock.Tick(now);
            WeExpectThat(ourGhost()).IsAt(startingLocation);
            await _gameClock.Tick(now);
            WeExpectThat(ourGhost()).IsAt(startingLocation.Left);

            await _gameClock.Tick(now.AddSeconds(_gameSettings.InitialScatterTimeInSeconds + 1));
            WeExpectThat(ourGhost()).IsAt(startingLocation.Left.Right);

            await _gameClock.Tick(now.AddSeconds(_gameSettings.InitialScatterTimeInSeconds + 2));
            WeExpectThat(ourGhost()).IsAt(startingLocation.Left.Right.Right);

            now = now.AddSeconds(_gameSettings.InitialScatterTimeInSeconds);
            await _gameClock.Tick(now.AddSeconds(_gameSettings.ChaseTimeInSeconds + 1));
            WeExpectThat(ourGhost()).IsAt(startingLocation.Left.Right.Right.Left);

            await _gameClock.Tick(now.AddSeconds(_gameSettings.ChaseTimeInSeconds + 2));

            ourGhost()
                .Should().BeEquivalentTo(new
                {
                    Location = new
                    {
                        X = 29,
                        Y = 1
                    }
                });


            Ghost ourGhost() => game.Ghosts.Values.First();
        }

        [Fact]
        public async Task GhostsDoNotStartOffAsEdible()
        {
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", _gameSettings.PacMan.Location.Right.Right, Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));
            _gameSettings.Ghosts.Add(new Ghost("Ghost2", _gameSettings.PacMan.Location.Right.Right, Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));
            _gameSettings.Ghosts.Add(new Ghost("Ghost3", _gameSettings.PacMan.Location.Right.Right, Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await _gameClock.Tick();

            game.Ghosts.Values.Should().AllBeEquivalentTo(new
            {
                Edible = false
            });
        }

        public CellLocation FarAway(CellLocation location) => new CellLocation(location.X + 10, location.Y + 10);

        [Theory]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(10)]
        public async Task AllGhostsShouldReturnToNonEdibleAfterGivenAmountOfSeconds(int seconds)
        {
            _gameSettings.FrightenedTimeInSeconds = seconds;
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", FarAway(_gameSettings.PacMan.Location), Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));
            _gameSettings.Ghosts.Add(new Ghost("Ghost2", FarAway(_gameSettings.PacMan.Location), Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));
            _gameSettings.Ghosts.Add(new Ghost("Ghost3", FarAway(_gameSettings.PacMan.Location), Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await game.ChangeDirection(Direction.Right);

            var now = DateTime.UtcNow;
            await _gameClock.Tick(now);

            if (!game.Ghosts.Values.All(g => g.Edible))
                throw new Exception("All ghosts are meant to be edible.");

            await _gameClock.Tick(now.AddSeconds(seconds).AddMilliseconds(-100));
            if (!game.Ghosts.Values.All(g => g.Edible))
                throw new Exception("All ghosts are meant to be edible.");

            await _gameClock.Tick(now.AddSeconds(seconds));

            game.Ghosts.Values.Should().AllBeEquivalentTo(new
            {
                Edible = false
            });
        }

        [Theory]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(10)]
        public async Task GhostShouldReturnToNonEdibleAfterGivenAmountSecondsWhenEatingSecondPowerPills(int seconds)
        {
            _gameSettings.FrightenedTimeInSeconds = seconds;
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", FarAway(_gameSettings.PacMan.Location), Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));
            _gameSettings.Ghosts.Add(new Ghost("Ghost2", FarAway(_gameSettings.PacMan.Location), Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));
            _gameSettings.Ghosts.Add(new Ghost("Ghost3", FarAway(_gameSettings.PacMan.Location), Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right);
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right.Right.Right);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await game.ChangeDirection(Direction.Right);

            var now = DateTime.UtcNow;
            await _gameClock.Tick(now);
            await _gameClock.Tick(now.AddSeconds(seconds - 1));
            await _gameClock.Tick(now.AddSeconds(seconds - 1));

            if (!game.Ghosts.Values.All(g => g.Edible))
                throw new Exception("All ghosts are meant to be edible.");

            await _gameClock.Tick(now.AddSeconds(seconds * 2 - 1).AddMilliseconds(-100));
            if (!game.Ghosts.Values.All(g => g.Edible))
                throw new Exception("All ghosts are meant to be edible.");

            await _gameClock.Tick(now.AddSeconds(seconds * 2 - 1));

            game.Ghosts.Values.Should().AllBeEquivalentTo(new
            {
                Edible = false
            });
        }

        [Fact]
        public async Task AllGhostsShouldRemainInLocationAfterPowerPillIsEaten()
        {
            var ghostStart = _gameSettings.PacMan.Location.Below.Right;
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", ghostStart, Direction.Left, CellLocation.TopLeft, new GhostGoesRightStrategy()));
            _gameSettings.Ghosts.Add(new Ghost("Ghost2", ghostStart, Direction.Left, CellLocation.TopLeft, new GhostGoesRightStrategy()));
            _gameSettings.Ghosts.Add(new Ghost("Ghost3", ghostStart, Direction.Left, CellLocation.TopLeft, new GhostGoesRightStrategy()));

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await game.ChangeDirection(Direction.Right);

            var now = DateTime.UtcNow;
            await _gameClock.Tick(now);

            if (!game.Ghosts.Values.All(g => g.Edible))
                throw new Exception("All ghosts are meant to be edible.");

            game.Ghosts.Values.Should().AllBeEquivalentTo(new
            {
                Location = ghostStart.Right
            });
        }


        [Fact]
        public async Task AllGhostsShouldShouldStayInSameLocationWhenTransitioningToNoneEdiable()
        {
            var ghostStart = _gameSettings.PacMan.Location.Below.Right;
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", ghostStart, Direction.Left, CellLocation.TopLeft, new GhostGoesRightStrategy()));
            _gameSettings.Ghosts.Add(new Ghost("Ghost2", ghostStart, Direction.Left, CellLocation.TopLeft, new GhostGoesRightStrategy()));
            _gameSettings.Ghosts.Add(new Ghost("Ghost3", ghostStart, Direction.Left, CellLocation.TopLeft, new GhostGoesRightStrategy()));

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await game.ChangeDirection(Direction.Right);

            var now = DateTime.UtcNow;
            await _gameClock.Tick(now);

            if (!game.Ghosts.Values.All(g => g.Edible))
                throw new Exception("All ghosts are meant to be edible.");

            await _gameClock.Tick(now.AddSeconds(7));

            if (!game.Ghosts.Values.All(g => !g.Edible))
                throw new Exception("All ghosts are meant to be nonedible.");

            using var _ = new AssertionScope();

            foreach (var ghost in game.Ghosts.Values)
            {
                ghost.Should().NotBeEquivalentTo(new
                {
                    Location = ghostStart
                });
            }
        }

        [Fact]
        public async Task GhostsGoHomeAfterCollidesWithPacManAfterEatingPowerPill()
        {
            var ghostStart1and3 = _gameSettings.PacMan.Location.Left.Left.Left;
            var ghostStart2 = ghostStart1and3.Below;
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", ghostStart1and3, Direction.Left, CellLocation.TopLeft, new GhostGoesRightStrategy()));
            _gameSettings.Ghosts.Add(new Ghost("Ghost2", ghostStart2, Direction.Left, CellLocation.TopLeft, new GhostGoesRightStrategy()));
            _gameSettings.Ghosts.Add(new Ghost("Ghost3", ghostStart1and3, Direction.Left, CellLocation.TopLeft, new GhostGoesRightStrategy()));

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await game.ChangeDirection(Direction.Left);

            var now = DateTime.UtcNow;
            await _gameClock.Tick(now);

            WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Left);
            WeExpectThat(game.Ghosts["Ghost1"]).IsAt(ghostStart1and3.Right);

            await _gameClock.Tick(now);
            WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Left.Left);

            game.Ghosts["Ghost1"].Should().BeEquivalentTo(new
            {
                Location = ghostStart1and3
            });

            game.Ghosts["Ghost2"].Should().NotBeEquivalentTo(new
            {
                Location = ghostStart2
            });

            game.Ghosts["Ghost3"].Should().BeEquivalentTo(new
            {
                Location = ghostStart1and3
            });
        }

        [Fact]
        public async Task TheEdibleGhostBecomesNonEdibleAfterBeingEaten()
        {
            var ghostStart1 = _gameSettings.PacMan.Location.Left.Left.Left;
            var ghostStart2 = _gameSettings.PacMan.Location.Below.Below.Below;
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", ghostStart1, Direction.Left, CellLocation.TopLeft, new GhostGoesRightStrategy()));
            _gameSettings.Ghosts.Add(new Ghost("Ghost2", ghostStart2, Direction.Left, CellLocation.TopLeft, new GhostGoesRightStrategy()));

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await game.ChangeDirection(Direction.Left);

            var now = DateTime.UtcNow;
            await _gameClock.Tick(now);

            WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Left);
            WeExpectThat(game.Ghosts["Ghost1"]).IsAt(ghostStart1.Right);

            await _gameClock.Tick(now);
            WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Left.Left);


            using var _ = new AssertionScope();
            game.Ghosts["Ghost1"].Should().BeEquivalentTo(new
            {
                Edible = false
            });

            game.Ghosts["Ghost2"].Should().BeEquivalentTo(new
            {
                Edible = true
            });

        }

        [Fact]
        public async Task TheEdibleGhostMovesAtHalfSpeedWhileFrightened()
        {
            var ghostStart1 = _gameSettings.PacMan.Location.Below.Below;
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", ghostStart1, Direction.Left, CellLocation.TopLeft, new GhostGoesRightStrategy()));

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await game.ChangeDirection(Direction.Left);

            await _gameClock.Tick();

            await _gameClock.Tick();
            await _gameClock.Tick();
            await _gameClock.Tick();
            await _gameClock.Tick();

            using var _ = new AssertionScope();
            game.Ghosts["Ghost1"].Should().BeEquivalentTo(new
            {
                Location = ghostStart1.Right.Right.Right
            });
        }

        [Fact]
        public async Task GhostMovesRandomlyWhileFrightened()
        {
            var testDirectionPicker = new TestDirectionPicker();
            testDirectionPicker.DefaultDirection = Direction.Up;
            var allDirections = new[] { Direction.Down, Direction.Up, Direction.Right };
            testDirectionPicker.Returns(allDirections, Direction.Right);

            _gameSettings.DirectionPicker = testDirectionPicker;
            var ghostStart1 = _gameSettings.PacMan.Location.Below.Below;
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", ghostStart1, Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            // PacMan Eat Pill
            await game.ChangeDirection(Direction.Left);
            await _gameClock.Tick();

            await _gameClock.Tick();
            await _gameClock.Tick();

            await _gameClock.Tick();
            await _gameClock.Tick();

            using var _ = new AssertionScope();
            game.Ghosts["Ghost1"].Should().BeEquivalentTo(new
            {
                Location = ghostStart1.Right.Right
            });
        }


        [Fact]
        public async Task GhostIsTeleportedWhenWalkingIntoAPortal()
        {
            CellLocation portalEntrance = (10, 1);
            CellLocation portalExit = (1, 5);
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", portalEntrance.Left, Direction.Left, CellLocation.TopLeft, new GhostGoesRightStrategy()));
            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            _gameSettings.Portals.Add(portalEntrance, portalExit);

            await _gameClock.Tick();

            game.Ghosts.Values.First()
                .Should()
                .BeEquivalentTo(new
                {
                    Location = portalExit.Right
                });
        }
    }
}
