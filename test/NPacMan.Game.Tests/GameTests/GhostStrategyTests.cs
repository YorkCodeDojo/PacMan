using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using NPacMan.Game.GhostStrategies;
using NPacMan.Game.Tests.GhostStrategiesForTests;
using Xunit;
using static NPacMan.Game.Tests.Helpers.Ensure;

namespace NPacMan.Game.Tests.GameTests
{
    public class GhostStrategyTests
    {
        private readonly GhostBuilder _ghostBuilder;

        public GhostStrategyTests()
        {
            _ghostBuilder = GhostBuilder.New()
                    .WithChaseStrategy(new DirectToStrategy(new DirectToPacManLocation()));
        }

        [Theory]
        [InlineData(0, 5, 4, 5)]
        [InlineData(5, 0, 5, 4)]
        [InlineData(5, 10, 5, 6)]
        public async Task ShouldMoveTowardsPacMan(int pacManX, int pacManY, int expectedGhostPositionX, int expectedGhostPositionY)
        {
            //pacManX: 10, pacManY: 5, expectedGhostPositionX: 6, expectedGhostPositionY: 5) [FAIL
            var ghost = _ghostBuilder.WithLocation((5, 5)).Create();
            var board = new TestGameSettings()
            {
                Ghosts = { ghost },
                PacMan = new PacMan((pacManX, pacManY), Direction.Left)
            };

            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);
            game.StartGame();
            await gameClock.Tick();

            game.Ghosts[ghost.Name].Should().BeEquivalentTo(new
            {
                Location = new
                {
                    X = expectedGhostPositionX,
                    Y = expectedGhostPositionY
                }
            });
        }

        [Fact]
        public async Task ShouldNotWalkInToWall()
        {
            var ghost = _ghostBuilder.WithLocation((5, 5)).Create();
            var board = new TestGameSettings()
            {
                Ghosts = { ghost },
                PacMan = new PacMan((3, 5), Direction.Left),
                Walls = { (4, 5) }
            };

            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);
            game.StartGame();
            await gameClock.Tick();

            using var _ = new AssertionScope();
            game.Ghosts[ghost.Name].Should().NotBeEquivalentTo(new
            {
                Location = new
                {
                    X = 4,
                    Y = 5
                }
            });

            game.Ghosts[ghost.Name].Should().NotBeEquivalentTo(new
            {
                Location = new
                {
                    X = 5,
                    Y = 5
                }
            });
        }


        [Fact]
        public async Task ShouldNotWalkInToWall2()
        {
            var ghost = _ghostBuilder.WithLocation((5, 5)).Create();
            var board = new TestGameSettings()
            {
                Ghosts = { ghost },
                PacMan = new PacMan((5, 3), Direction.Left),
                Walls = { (5, 4) }
            };

            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);
            game.StartGame();
            await gameClock.Tick();

            using var _ = new AssertionScope();
            game.Ghosts[ghost.Name].Should().NotBeEquivalentTo(new
            {
                Location = new
                {
                    X = 5,
                    Y = 4
                }
            });

            game.Ghosts[ghost.Name].Should().NotBeEquivalentTo(new
            {
                Location = new
                {
                    X = 5,
                    Y = 5
                }
            });
        }

        [Fact]
        public async Task ShouldTurnAtCorner()
        {
            // X X X X 
            // X ^ ▶ ▶
            // X S X X

            var ghost = _ghostBuilder.WithLocation((1, 2))
                .WithDirection(Direction.Up).Create();
            var board = new TestGameSettings()
            {
                Ghosts = { ghost },
                PacMan = new PacMan((4, 1), Direction.Right),
                Walls = { (0, 0), (1, 0), (2, 0), (3, 0),
                    (0,1), (0,2), (2,2), (3,2)
                }
            };

            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);
            game.StartGame();
            await gameClock.Tick();
            await gameClock.Tick();
            await gameClock.Tick();

            using var _ = new AssertionScope();
            game.Ghosts[ghost.Name].Should().BeEquivalentTo(new
            {
                Location = new
                {
                    X = 3,
                    Y = 1
                },
                Direction = Direction.Right
            });
        }

        [Fact]
        public async Task ShouldTurnTowardsPacManAtJunction()
        {
            // X   X X 
            //   ^ ▶ ▶ C
            // X S X X

            var ghost = _ghostBuilder.WithLocation((1, 2))
                .WithDirection(Direction.Up).Create();
            var board = new TestGameSettings()
            {
                Ghosts = { ghost },
                PacMan = new PacMan((4, 1), Direction.Right),
                Walls = { (0, 0), (2, 0), (3, 0),
                   (0,2), (2,2), (3,2)
                }
            };

            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);
            game.StartGame();
            await gameClock.Tick();
            await gameClock.Tick();
            await gameClock.Tick();

            using var _ = new AssertionScope();
            game.Ghosts[ghost.Name].Should().BeEquivalentTo(new
            {
                Location = new
                {
                    X = 3,
                    Y = 1
                },
                Direction = Direction.Right
            });
        }


        [Fact]
        public void ShouldBeAbleToTargetWherePacManWillBe()
        {
            var board = new TestGameSettings()
            {
                PacMan = new PacMan((4, 1), Direction.Right),
                Walls = { }
            };
            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);
            game.StartGame();
            var directToExpectedPacManLocation = new DirectToExpectedPacManLocation();
            var targetLocation = directToExpectedPacManLocation.GetLocation(game);

            targetLocation.Should().BeEquivalentTo(new
            {
                X = 8,
                Y = 1
            });
        }

        [Fact]
        public void ShouldBeAbleInterceptPacMan()
        {
            // ghost = 1,2
            // pacman (+2) will be at= 6,1
            // difference 5,-1
            // target vector = (difference * 2) = 10, -2
            // ghost + vector = 11, 0

            var ghost = _ghostBuilder.WithLocation((1, 2))
                .WithChaseStrategy(new NullGhostStrategy()).Create();
            var board = new TestGameSettings()
            {
                PacMan = new PacMan((4, 1), Direction.Right),
                Ghosts = { ghost },
                Walls = { }
            };
            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);
            game.StartGame();
            var inkyTargetCell = new InterceptPacManLocation(ghost.Name);
            var targetLocation = inkyTargetCell.GetLocation(game);

            targetLocation.Should().BeEquivalentTo(new
            {
                X = 11,
                Y = 0
            });
        }

        [Fact]
        public void StayCloseToPacManMovesTowardsHimWhileOutsideOf8Cells()
        {
            var ghost = _ghostBuilder.WithLocation((1, 2))
                .WithChaseStrategy(new NullGhostStrategy()).Create();
            var board = new TestGameSettings()
            {
                PacMan = new PacMan((14, 1), Direction.Right),
                Walls = { },
                Ghosts = { ghost },
            };
            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);
            game.StartGame();
            var staysCloseToPacManLocation = new StaysCloseToPacManLocation(ghost.Name);
            var targetLocation = staysCloseToPacManLocation.GetLocation(game);

            targetLocation.Should().BeEquivalentTo(new
            {
                X = 14,
                Y = 1
            });
        }

        [Fact]
        public void StayCloseToPacManScattersWhileInsideOf8Cells()
        {
            var ghost = _ghostBuilder.WithLocation((1, 2))
                .WithChaseStrategy(new NullGhostStrategy())
                .WithScatterTarget((11, 12))
                .Create();
            var board = new TestGameSettings()
            {
                PacMan = new PacMan((4, 1), Direction.Right),
                Walls = { },
                Ghosts = { ghost },
            };
            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);
            game.StartGame();
            var staysCloseToPacManLocation = new StaysCloseToPacManLocation(ghost.Name);
            var targetLocation = staysCloseToPacManLocation.GetLocation(game);

            targetLocation.Should().BeEquivalentTo(new
            {
                X = 11,
                Y = 12
            });
        }
    }
}
