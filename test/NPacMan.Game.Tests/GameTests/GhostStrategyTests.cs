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
                                        .WithScatterStrategy(new StandingStillGhostStrategy())
                                        .WithChaseStrategy(new DirectToStrategy(new DirectToPacManLocation()));

            _gameSettings = new TestGameSettings();
        }

        [Theory]
        [InlineData(0, 5, 4, 5)]
        [InlineData(5, 0, 5, 4)]
        [InlineData(5, 10, 5, 6)]
        public async Task ShouldMoveTowardsPacMan(int pacManX, int pacManY, int expectedGhostPositionX, int expectedGhostPositionY)
        {
            var ghost = _ghostBuilder.WithLocation((5, 5)).Create();
            _gameSettings.Ghosts.Add(ghost);
            _gameSettings.PacMan = new PacMan((pacManX, pacManY), Direction.Left);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();
            await gameHarness.WaitForScatterToComplete();

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
            await gameHarness.PlayGame();
            await gameHarness.WaitForScatterToComplete();

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
        public async Task ShouldNotWalkVerticallyInToWalls()
        {
            var wallLocation = _gameSettings.PacMan.Location.Below;
            var ghostStartLocation = _gameSettings.PacMan.Location.Below.Below;

            _gameSettings.Walls.Add(wallLocation);
            var ghost = _ghostBuilder.WithLocation(ghostStartLocation).Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();
            await gameHarness.WaitForScatterToComplete();

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
            // X ^ ▶ ▶ C
            // X S X X

            var topLeft = new CellLocation();
            _gameSettings.Walls.Add(topLeft);
            _gameSettings.Walls.Add(topLeft.Right);
            _gameSettings.Walls.Add(topLeft.Right.Right);
            _gameSettings.Walls.Add(topLeft.Right.Right.Right);
            _gameSettings.Walls.Add(topLeft.Below);
            _gameSettings.Walls.Add(topLeft.Below.Below);
            _gameSettings.Walls.Add(topLeft.Below.Below.Right.Right);
            _gameSettings.Walls.Add(topLeft.Below.Below.Right.Right.Right);

            var ghost = _ghostBuilder.WithLocation(topLeft.Below.Below.Right)
                                     .WithDirection(Direction.Up).Create();
            _gameSettings.Ghosts.Add(ghost);

            _gameSettings.PacMan = new PacMan(topLeft.Below.Right.Right.Right.Right, Direction.Right);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();
            await gameHarness.WaitForScatterToComplete();
            await gameHarness.Move();
            await gameHarness.Move();

            using var _ = new AssertionScope();
            gameHarness.Game.Ghosts[ghost.Name].Should().BeEquivalentTo(new
            {
                Location = topLeft.Below.Right.Right.Right,
                Direction = Direction.Right
            });
        }

        [Fact]
        public async Task ShouldTurnTowardsPacManAtJunction()
        {
            // X   X X 
            //   ^ ▶ ▶ C
            // X S X X

            var topLeft = new CellLocation();
            _gameSettings.Walls.Add(topLeft);
            _gameSettings.Walls.Add(topLeft.Right.Right);
            _gameSettings.Walls.Add(topLeft.Right.Right.Right);
            _gameSettings.Walls.Add(topLeft.Below.Below);
            _gameSettings.Walls.Add(topLeft.Below.Below.Right.Right);
            _gameSettings.Walls.Add(topLeft.Below.Below.Right.Right.Right);

            var ghost = _ghostBuilder.WithLocation(topLeft.Below.Below.Right)
                                        .WithDirection(Direction.Up).Create();
            _gameSettings.Ghosts.Add(ghost);

            _gameSettings.PacMan = new PacMan(topLeft.Below.Right.Right.Right.Right, Direction.Right);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.WaitForScatterToComplete();
            await gameHarness.Move();
            await gameHarness.Move();

            using var _ = new AssertionScope();
            gameHarness.Game.Ghosts[ghost.Name].Should().BeEquivalentTo(new
            {
                Location = topLeft.Below.Right.Right.Right,
                Direction = Direction.Right
            });
        }

        [Fact]
        public async Task ShouldBeAbleToTargetWherePacManWillBe()
        {
            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            var directToExpectedPacManLocation = new DirectToExpectedPacManLocation();
            var targetLocation = directToExpectedPacManLocation.GetLocation(gameHarness.Game);

            targetLocation.Should().BeEquivalentTo(new
            {
                _gameSettings.PacMan.Location.Right.Right.Right.Right.Right
            });
        }

        [Fact]
        public async Task ShouldBeAbleInterceptPacMan()
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
            await gameHarness.PlayGame();

            var inkyTargetCell = new InterceptPacManLocation(ghost.Name);
            var targetLocation = inkyTargetCell.GetLocation(gameHarness.Game);

            targetLocation.Should().BeEquivalentTo(new
            {
                X = 11,
                Y = 0
            });
        }

        [Fact]
        public async Task StayCloseToPacManMovesTowardsHimWhileOutsideOf8Cells()
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
            await gameHarness.PlayGame();

            var staysCloseToPacManLocation = new StaysCloseToPacManLocation(ghost.Name, CellLocation.TopLeft);
            var targetLocation = staysCloseToPacManLocation.GetLocation(gameHarness.Game);

            targetLocation.Should().BeEquivalentTo(new
            {
                X = 14,
                Y = 1
            });
        }

        [Fact]
        public async Task StayCloseToPacManScattersWhileInsideOf8Cells()
        {
            var scatterTarget = (11, 12);
            var ghost = _ghostBuilder.WithLocation((1, 2))
                .WithChaseStrategy(new NullGhostStrategy())
                .WithScatterTarget(scatterTarget)
                .Create();
            var board = new TestGameSettings()
            {
                PacMan = new PacMan((4, 1), Direction.Right),
                Walls = { },
                Ghosts = { ghost },
            };

            var gameHarness = new GameHarness(board);
            await gameHarness.PlayGame();

            var staysCloseToPacManLocation = new StaysCloseToPacManLocation(ghost.Name, scatterTarget);
            var targetLocation = staysCloseToPacManLocation.GetLocation(gameHarness.Game);

            targetLocation.Should().BeEquivalentTo(new
            {
                X = 11,
                Y = 12
            });
        }
    }
}
