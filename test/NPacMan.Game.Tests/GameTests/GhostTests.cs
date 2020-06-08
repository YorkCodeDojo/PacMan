using System;
using System.Dynamic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using GreenPipes;
using NPacMan.Game.GhostStrategies;
using NPacMan.Game.Tests.Helpers;
using Xunit;
using static NPacMan.Game.Tests.Helpers.Ensure;

namespace NPacMan.Game.Tests.GameTests
{
    public class GhostTests
    {
        private readonly TestGameSettings _gameSettings;
        private readonly TestGameClock _gameClock;

        public GhostTests()
        {
            _gameSettings = new TestGameSettings{
                PowerPills = { new CellLocation(50,50) }
            };
            _gameClock = new TestGameClock();
        }

        [Fact]
        public async Task GhostMovesInDirectionOfStrategy()
        {
            var ghost = GhostBuilder.New()
                .WithChaseStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();
            await _gameClock.Tick();
            game.Ghosts[ghost.Name].Should().BeEquivalentTo(new
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
            var ghost = GhostBuilder.New()
                    .WithLocation((x, y))
                    .WithChaseStrategy(new DirectToStrategy(new DirectToPacManLocation()))
                    .Create();
            _gameSettings.Ghosts.Add(ghost);
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
            var ghost = GhostBuilder.New()
                .WithLocation(homeLocation)
                .WithDirection(Direction.Right)
                .WithChaseStrategy(new DirectToStrategy(new DirectToPacManLocation()))
                .Create();

            _gameSettings.Ghosts.Add(ghost);

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
            var ghost = GhostBuilder.New()
                .WithLocation(homeLocation)
                .WithDirection(Direction.Right)
                .WithChaseStrategyRight()
                .Create();
            
            _gameSettings.Ghosts.Add(ghost);

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

            var ghost = GhostBuilder.New()
                .WithLocation(startingLocation)
                .WithScatterTarget(scatterLocation)
                .WithDirection(Direction.Right)
                .WithChaseStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost);

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

            var ghost = GhostBuilder.New()
                .WithLocation(startingLocation)
                .WithScatterTarget(scatterLocation)
                .WithDirection(Direction.Right)
                .WithChaseStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost);

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

            var ghost = GhostBuilder.New()
                .WithLocation(startingLocation)
                .WithScatterTarget(scatterLocation)
                .WithDirection(Direction.Right)
                .WithChaseStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost);

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

            var ghosts = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.Right.Right)
                .CreateMany(3);
            _gameSettings.Ghosts.AddRange(ghosts);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await _gameClock.Tick();

            game.Ghosts.Values.Should().AllBeEquivalentTo(new
            {
                Edible = false
            });
        }


        [Theory]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(10)]
        public async Task AllGhostsShouldReturnToNonEdibleAfterGivenAmountOfSeconds(int seconds)
        {
            _gameSettings.FrightenedTimeInSeconds = seconds;
            var ghosts = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .CreateMany(3);
            _gameSettings.Ghosts.AddRange(ghosts);

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
            var ghosts = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .CreateMany(3);
            _gameSettings.Ghosts.AddRange(ghosts);
            
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
            var ghosts = GhostBuilder.New()
                .WithLocation(ghostStart)
                .CreateMany(3);
            _gameSettings.Ghosts.AddRange(ghosts);

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
                Location = ghostStart
            });
        }


        [Fact]
        public async Task AllGhostsShouldShouldStayInSameLocationWhenTransitioningToNoneEdible()
        {
            var ghostStart = _gameSettings.PacMan.Location.Below.Right;
            var ghosts = GhostBuilder.New()
                .WithLocation(ghostStart)
                .WithChaseStrategyRight()
                .CreateMany(3);

            _gameSettings.Ghosts.AddRange(ghosts);
            
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
            var haunt1 = GhostBuilder.New()
                .WithLocation(ghostStart1and3)
                .WithChaseStrategyRight()
                .CreateMany(2);
            var haunt2 = GhostBuilder.New()
                .WithLocation(ghostStart2)
                .WithChaseStrategyRight()
                .CreateMany(1);

            _gameSettings.Ghosts.AddRange(haunt1.Concat(haunt2));

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await game.ChangeDirection(Direction.Left);

            var now = DateTime.UtcNow;
            await _gameClock.Tick(now);

            WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Left);
            foreach (var g in haunt1)
            {
                WeExpectThat(game.Ghosts[g.Name]).IsAt(ghostStart1and3.Right);
            }

            await _gameClock.Tick(now);
            WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Left.Left);

            game.Ghosts[haunt1[0].Name].Should().BeEquivalentTo(new
            {
                Location = ghostStart1and3
            });

            game.Ghosts[haunt2.First().Name].Should().NotBeEquivalentTo(new
            {
                Location = ghostStart2
            });

            game.Ghosts[haunt1[1].Name].Should().BeEquivalentTo(new
            {
                Location = ghostStart1and3
            });
        }

        [Fact]
        public async Task TheEdibleGhostBecomesNonEdibleAfterBeingEaten()
        {
            var ghostStart1 = _gameSettings.PacMan.Location.Left.Left.Left;
            var ghostStart2 = _gameSettings.PacMan.Location.Below.Below.Below;
            var ghost1 = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .WithChaseStrategyRight()
                .Create();
            var ghost2 = GhostBuilder.New()
                .WithLocation(ghostStart2)
                .WithChaseStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost1);
            _gameSettings.Ghosts.Add(ghost2);

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await game.ChangeDirection(Direction.Left);

            var now = DateTime.UtcNow;
            await _gameClock.Tick(now);

            WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Left);
            WeExpectThat(game.Ghosts[ghost1.Name]).IsAt(ghostStart1.Right);

            await _gameClock.Tick(now);
            WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Left.Left);


            using var _ = new AssertionScope();
            game.Ghosts[ghost1.Name].Should().BeEquivalentTo(new
            {
                Edible = false
            });

            game.Ghosts[ghost2.Name].Should().BeEquivalentTo(new
            {
                Edible = true
            });

        }

        [Fact]
        public async Task TheEdibleGhostMovesAtHalfSpeedWhileFrightened()
        {
            var ghostStart1 = _gameSettings.PacMan.Location.Below.Below;
            var ghost1 = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .WithChaseStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost1);

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
            game.Ghosts[ghost1.Name].Should().BeEquivalentTo(new
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
            var ghostStart1 = _gameSettings.PacMan.Location.FarAway();
            var ghost1 = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .Create();
            _gameSettings.Ghosts.Add(ghost1);

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
            game.Ghosts[ghost1.Name].Should().BeEquivalentTo(new
            {
                Location = ghostStart1.Right.Right
            });
        }


        [Fact]
        public async Task GhostIsTeleportedWhenWalkingIntoAPortal()
        {
            var portalEntrance = new CellLocation(10, 1);
            var portalExit = new CellLocation(1, 5);
            var ghost1 = GhostBuilder.New()
                .WithLocation(portalEntrance.Left)
                .WithChaseStrategyRight()
                .Create();

            _gameSettings.Ghosts.Add(ghost1);
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

        [Fact]
        public async Task TheEdibleGhostReturnsToStrategyAfterBeingEaten()
        {
            var ghostStart1 = _gameSettings.PacMan.Location.Above.Above.Left;
            var ghost1 = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .Create();
            _gameSettings.Ghosts.Add(ghost1);
           
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Above);

            // . . .
            // . G .
            // . . *
            // . . V

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await game.ChangeDirection(Direction.Up);

            var now = DateTime.UtcNow;
            await _gameClock.Tick(now);

            // . . .
            // . G .
            // . . V

            WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Above);
            WeExpectThat(game.Ghosts[ghost1.Name]).IsAt(ghostStart1);

            await _gameClock.Tick(now);

            // . . .
            // . . GV

            // Perhaps . G V ????

            WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Above.Above);
            WeExpectThat(game.Ghosts[ghost1.Name]).IsAt(ghostStart1);

            await _gameClock.Tick(now);

            // . . V
            // . G .
            // . . .

            WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Above.Above.Above);

            using var _ = new AssertionScope();
            game.Ghosts[ghost1.Name].Should().BeEquivalentTo(new
            {
                Location = ghostStart1
            });
        }

        
        [Fact]
        public async Task GhostsShouldStandStillInGhostHouse()
        {
            var ghostStart1 = _gameSettings.PacMan.Location.FarAway();
            _gameSettings.GhostHouse.Add(ghostStart1);
            var ghosts = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .WithNumberOfCoinsRequiredToExitHouse(int.MaxValue)
                .CreateMany(2);
            _gameSettings.Ghosts.AddRange(ghosts);
            
            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await _gameClock.Tick();
            await _gameClock.Tick();

            game.Ghosts.Values.Should().AllBeEquivalentTo(new {
                Location = ghostStart1
            });
        }

        [Fact]
        public async Task GhostsShouldLeaveGhostHouseWhenPillCountHasBeenReached()
        {
            // X
            // -
            // H G

            var ghostStart1 = new CellLocation(1, 2);
            _gameSettings.PacMan = new PacMan(ghostStart1.FarAway(), Direction.Left);
            _gameSettings.GhostHouse.AddRange(new [] {ghostStart1, ghostStart1.Left});
            _gameSettings.Doors.Add(ghostStart1.Left.Above);
            var ghost1 = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .WithNumberOfCoinsRequiredToExitHouse(1)
                .Create();
            var ghost2 = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .WithNumberOfCoinsRequiredToExitHouse(10)
                .Create();
            _gameSettings.Ghosts.Add(ghost1);
            _gameSettings.Ghosts.Add(ghost2);
           
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.FarAway());
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();
            await game.ChangeDirection(Direction.Left);

            // PacMan Eats Coin
            await _gameClock.Tick();

            // Still in House, under door
            await _gameClock.Tick();

            // On ghost door
            await _gameClock.Tick();

            // Out of house
            await _gameClock.Tick();

            using var _  = new AssertionScope();
            game.Ghosts[ghost1.Name].Should().BeEquivalentTo(new {
                Location = ghostStart1.Left.Above.Above
            });
             game.Ghosts[ghost2.Name].Should().BeEquivalentTo(new {
                Location = ghostStart1
            });
        }

        [Fact]
        public async Task GhostsShouldBeNotEdibleAfterPacManComesBackToLife(){
            //     G
            // > .   #       G
            var ghost1 = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.Right.Right.Above)
                .Create();
            var ghost2 = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .Create();
            var directionPicker = new TestDirectionPicker(){DefaultDirection = Direction.Down};
            _gameSettings.DirectionPicker = directionPicker;
            _gameSettings.Ghosts.Add(ghost1);
            _gameSettings.Ghosts.Add(ghost2);
            _gameSettings.Walls.Add(_gameSettings.PacMan.Location.Right.Right.Right);
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right);

            var now = DateTime.UtcNow;
            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            // Eat PowerPill
            await _gameClock.Tick(now);

            WeExpectThat(game.Ghosts[ghost1.Name]).IsEdible();
            WeExpectThat(game.Ghosts[ghost2.Name]).IsEdible();

            var score = game.Score;

            await _gameClock.Tick(now);
            await _gameClock.Tick(now);


            WeExpectThat(game.Ghosts[ghost1.Name]).IsAt(_gameSettings.PacMan.Location.Right.Right.Above);

            if(score == game.Score)
            {
                throw new Exception("Score should increase as ghost is eaten");
            }

            WeExpectThat(game.Ghosts[ghost1.Name]).IsNotEdible();
            WeExpectThat(game.Ghosts[ghost2.Name]).IsEdible();
            
            await game.ChangeDirection(Direction.Up);
            await _gameClock.Tick(now);

            if (game.Status != GameStatus.Dying)
                throw new Exception($"Invalid Game State {game.Status:G} Should be Dying");
            
            await _gameClock.Tick(now.AddSeconds(4));

            if (game.Status != GameStatus.Respawning)
                throw new Exception($"Invalid Game State {game.Status:G} Should be Respawning");

            await _gameClock.Tick(now.AddSeconds(8));

            if (game.Status != GameStatus.Alive)
                throw new Exception($"Invalid Game State {game.Status:G} Should be Alive");

            game.Ghosts.Values.Should().AllBeEquivalentTo(new{
                Edible = false
            });
        }

        [Fact]
        public async Task ScoreShouldIncreaseExponentiallyAfterEatingEachGhost()
        {
            var ghostStart = _gameSettings.PacMan.Location.Left.Left.Left;
            var ghost1 = GhostBuilder.New().WithLocation(ghostStart).WithChaseStrategyRight()
                .Create();
            
            _gameSettings.Ghosts.Add(ghost1);
            
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left);

            var game = new Game(_gameClock, _gameSettings);
            var numberOfNotificationsTriggered = 0;
            game.Subscribe(GameNotification.EatGhost, () => numberOfNotificationsTriggered++);            
            game.StartGame();

            await game.ChangeDirection(Direction.Left);

            await _gameClock.Tick();

            WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Left);
           
            WeExpectThat(game.Ghosts.Values).IsAt(ghostStart.Right);

            if (numberOfNotificationsTriggered != 0)
            {
                throw new Exception($"No EatGhost notifications should have been fired but {numberOfNotificationsTriggered} were.");
            }

            await _gameClock.Tick();

            WeExpectThat(game.PacMan).IsAt(_gameSettings.PacMan.Location.Left.Left);

            numberOfNotificationsTriggered.Should().Be(1);
        }


          [Fact]
        public async Task GhostsShouldBeInScatterModeAfterPacManComesBackToLife(){
            //   G         
            // > .       
            var ghost = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.Right.Above)
                .WithScatterTarget(_gameSettings.PacMan.Location.Right.Above.Above)
                .Create();

            var directionPicker = new TestDirectionPicker(){DefaultDirection = Direction.Down};
            _gameSettings.DirectionPicker = directionPicker;
            _gameSettings.Ghosts.Add(ghost);

            var now = DateTime.UtcNow;
            var game = new Game(_gameClock, _gameSettings);
            game.StartGame();

            await game.ChangeDirection(Direction.Right);
            await _gameClock.Tick(now);

            await game.ChangeDirection(Direction.Up);
            await _gameClock.Tick(now);   //Now dead

            if (game.Status != GameStatus.Dying)
                throw new Exception($"Invalid Game State {game.Status:G} Should be Dying");
            
            await _gameClock.Tick(now.AddSeconds(4));

            if (game.Status != GameStatus.Respawning)
                throw new Exception($"Invalid Game State {game.Status:G} Should be Respawning");

            await _gameClock.Tick(now.AddSeconds(8));

            if (game.Status != GameStatus.Alive)
                throw new Exception($"Invalid Game State {game.Status:G} Should be Alive");

            await _gameClock.Tick(now.AddSeconds(9));

            game.Ghosts.Values.Should().AllBeEquivalentTo(new{
                Edible = false,
                Location = _gameSettings.PacMan.Location.Right.Above.Above
            });
        }

    }
}



