using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using NPacMan.Game.GhostStrategies;
using NPacMan.Game.Tests.GhostStrategiesForTests;
using Xunit;

namespace NPacMan.Game.Tests.GameTests
{
    public class GhostStrategyTests
    {
        private readonly GhostBuilder _ghostBuilder;
        private readonly TestGameSettings _gameSettings;

        public GhostStrategyTests()
        {
            _ghostBuilder = GhostBuilder.New()
                                        .WithChaseStrategy(new DirectToStrategy(new DirectToPacManLocation()));
            
            _gameSettings = new TestGameSettings();
        }

        [Theory]
        [InlineData(0, 5, 4, 5)]
        [InlineData(5, 0, 5, 4)]
        [InlineData(5, 10, 5, 6)]
        public async Task ShouldMoveTowardsPacMan(int pacManX, int pacManY, int expectedGhostPositionX, int expectedGhostPositionY)
        {
            //pacManX: 10, pacManY: 5, expectedGhostPositionX: 6, expectedGhostPositionY: 5)
            var ghost = _ghostBuilder.WithLocation((5, 5)).Create();
            _gameSettings.Ghosts.Add(ghost);
            _gameSettings.PacMan = new PacMan((pacManX, pacManY), Direction.Left);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();
            await gameHarness.Move();

            gameHarness.Game.Ghosts[ghost.Name].Should().BeEquivalentTo(new
            {
                Location = new
                {
                    X = expectedGhostPositionX,
                    Y = expectedGhostPositionY
                }
            });
        }

        [Fact]
        public async Task ShouldNotWalkHorizontallyInToWalls()
        {
            var wallLocation = _gameSettings.PacMan.Location.Right;
            var ghostStartLocation = _gameSettings.PacMan.Location.Right.Right;

            _gameSettings.Walls.Add(wallLocation);
            var ghost = _ghostBuilder.WithLocation(ghostStartLocation).Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();
            await gameHarness.Move();

            using var _ = new AssertionScope();
            gameHarness.Game.Ghosts[ghost.Name].Should().NotBeEquivalentTo(new
            {
                Location = wallLocation
            });

            gameHarness.Game.Ghosts[ghost.Name].Should().NotBeEquivalentTo(new
            {
                Location = ghostStartLocation
            });
        }

        [Fact]
        public async Task ShouldNotVerticallyWalkInToWalls()
        {
            var wallLocation = _gameSettings.PacMan.Location.Below;
            var ghostStartLocation = _gameSettings.PacMan.Location.Below.Below;

            _gameSettings.Walls.Add(wallLocation);
            var ghost = _ghostBuilder.WithLocation(ghostStartLocation).Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();
            await gameHarness.Move();

            using var _ = new AssertionScope();
            gameHarness.Game.Ghosts[ghost.Name].Should().NotBeEquivalentTo(new
            {
                Location = wallLocation
            });

            gameHarness.Game.Ghosts[ghost.Name].Should().NotBeEquivalentTo(new
            {
                Location = ghostStartLocation
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

            var gameHarness = new GameHarness(board);
            gameHarness.StartGame();
            await gameHarness.Move();
            await gameHarness.Move();
            await gameHarness.Move();

            using var _ = new AssertionScope();
            gameHarness.Game.Ghosts[ghost.Name].Should().BeEquivalentTo(new
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

            var gameHarness = new GameHarness(board);
            gameHarness.StartGame();
            await gameHarness.Move();
            await gameHarness.Move();
            await gameHarness.Move();

            using var _ = new AssertionScope();
            gameHarness.Game.Ghosts[ghost.Name].Should().BeEquivalentTo(new
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
            
            var gameHarness = new GameHarness(board);
            gameHarness.StartGame();

            var directToExpectedPacManLocation = new DirectToExpectedPacManLocation();
            var targetLocation = directToExpectedPacManLocation.GetLocation(gameHarness.Game);

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

            var gameHarness = new GameHarness(board);
            gameHarness.StartGame();

            var inkyTargetCell = new InterceptPacManLocation(ghost.Name);
            var targetLocation = inkyTargetCell.GetLocation(gameHarness.Game);

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

            var gameHarness = new GameHarness(board);
            gameHarness.StartGame();

            var staysCloseToPacManLocation = new StaysCloseToPacManLocation(ghost.Name);
            var targetLocation = staysCloseToPacManLocation.GetLocation(gameHarness.Game);

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

            var gameHarness = new GameHarness(board);
            gameHarness.StartGame();

            var staysCloseToPacManLocation = new StaysCloseToPacManLocation(ghost.Name);
            var targetLocation = staysCloseToPacManLocation.GetLocation(gameHarness.Game);

            targetLocation.Should().BeEquivalentTo(new
            {
                X = 11,
                Y = 12
            });
        }
    }
}
