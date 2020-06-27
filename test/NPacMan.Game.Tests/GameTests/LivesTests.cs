using System.Threading.Tasks;
using FluentAssertions;
using NPacMan.Game.Tests.GhostStrategiesForTests;
using Xunit;

namespace NPacMan.Game.Tests.GameTests
{
    public class LivesTests
    {
        private readonly TestGameSettings _gameSettings;

        public LivesTests()
        {
            _gameSettings = new TestGameSettings();
        }

        [Fact]
        public async Task LivesStayTheSameWhenNotCollidingWithAGhost()
        {
            var ghost = GhostBuilder.New()
                                    .WithLocation(_gameSettings.PacMan.Location.Right)
                                    .WithScatterStrategy(new StandingStillGhostStrategy())
                                    .Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();
            var currentLives = gameHarness.Lives;

            await gameHarness.ChangeDirection(Direction.Left);
            await gameHarness.Move();

            gameHarness.Lives.Should().Be(currentLives);
        }

        [Fact]
        public async Task LivesDecreaseByOneWhenCollidesWithGhost()
        {
            var ghost = GhostBuilder.New()
                                    .WithLocation(_gameSettings.PacMan.Location.Right)
                                    .WithScatterStrategy(new StandingStillGhostStrategy())
                                    .Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();
            var currentLives = gameHarness.Lives;

            await gameHarness.ChangeDirection(Direction.Right);
            await gameHarness.GetEatenByGhost(ghost);

            gameHarness.Lives.Should().Be(currentLives - 1);
            gameHarness.Status.Should().Be(GameStatus.Dying);
        }


        [Fact]
        public async Task LivesDecreaseWhenCollidesWithGhostWalkingTowardsPacMan()
        {
            // G . . . P
            // . G . P .
            // . . PG . .
            var ghost = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.Left.Left.Left.Left)
                .WithChaseStrategyRight()
                .WithScatterStrategyRight()
                .Create();

            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();
            var currentLives = gameHarness.Lives;

            await gameHarness.ChangeDirection(Direction.Left);
            await gameHarness.Move();
            await gameHarness.GetEatenByGhost(ghost);

            gameHarness.Lives.Should().Be(currentLives - 1);
        }

        [Fact]
        public async Task LivesDecreaseWhenCollidesWithGhostWhenPacManIsFacingAWall()
        {
            _gameSettings.PacMan = _gameSettings.PacMan.WithNewDirection(Direction.Up);
            _gameSettings.Walls.Add(_gameSettings.PacMan.Location.Above);
            var ghost = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.Left)
                .WithChaseStrategyRight()
                .WithScatterStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();
            var currentLives = gameHarness.Lives;

            await gameHarness.GetEatenByGhost(ghost);

            gameHarness.Lives.Should().Be(currentLives - 1);
        }

        [Fact]
        public async Task ShouldNotLoseLifeWhenAlreadyIsDying()
        {
            var ghost = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.Left)
                .WithScatterStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost);

            var ghost2 = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.Left.Left)
                .WithScatterStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost2);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Left);

            await gameHarness.GetEatenByGhost(ghost);

            gameHarness.WeExpectThatPacMan().HasLives(_gameSettings.InitialLives - 1);

            gameHarness.EnsureGameStatus(GameStatus.Dying);

            await gameHarness.NOP();

            gameHarness.EnsureGameStatus(GameStatus.Dying);

            gameHarness.Lives.Should().Be(_gameSettings.InitialLives - 1);
        }
    }
}
