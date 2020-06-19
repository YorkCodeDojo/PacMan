using System.Threading.Tasks;
using FluentAssertions;
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
            var ghost = GhostBuilder.New().WithLocation(_gameSettings.PacMan.Location.Right).Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();
            var currentLives = gameHarness.Lives;

            await gameHarness.ChangeDirection(Direction.Left);
            await gameHarness.Move();

            gameHarness.Lives.Should().Be(currentLives);
        }

        [Fact]
        public async Task LivesDecreaseByOneWhenCollidesWithGhost()
        {
            var ghost = GhostBuilder.New().WithLocation(_gameSettings.PacMan.Location.Right).Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();
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
                .WithChaseStrategyRight().Create();

            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();
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
                .WithChaseStrategyRight().Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();
            var currentLives = gameHarness.Lives;

            await gameHarness.GetEatenByGhost(ghost);

            gameHarness.Lives.Should().Be(currentLives - 1);
        }

        [Fact]
        public async Task ShouldNotLoseLifeWhenAlreadyIsDying()
        {
            var expectedLife = 1;

            _gameSettings.InitialGameStatus = GameStatus.Dying;
            _gameSettings.InitialLives = expectedLife;
            _gameSettings.PacMan = _gameSettings.PacMan.WithNewDirection(Direction.Down);

            var ghost = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location)
                .Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            gameHarness.StartGame();
            await gameHarness.NOP();

            gameHarness.Lives.Should().Be(expectedLife);
        }
    }
}
