using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace NPacMan.Game.Tests.GameTests
{
    public class LivesTests
    {
        private readonly TestGameSettings _gameSettings;
        private readonly TestGameClock _gameClock;

        public LivesTests()
        {
            _gameSettings = new TestGameSettings();
            _gameClock = new TestGameClock();
        }

        [Fact]
        public async Task LivesStayTheSameWhenNotCollidingWithAGhost()
        {
            var ghost = GhostBuilder.New().WithLocation(_gameSettings.PacMan.Location.Right).Create();
            _gameSettings.Ghosts.Add(ghost);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame(); 
            var currentLives = game.Lives;

            await game.ChangeDirection(Direction.Left);
            await _gameClock.Tick();

            game.Lives.Should().Be(currentLives);
        }

        [Fact]
        public async Task LivesDecreaseByOneWhenCollidesWithGhost()
        {
            var ghost = GhostBuilder.New().WithLocation(_gameSettings.PacMan.Location.Right).Create();

            _gameSettings.Ghosts.Add(ghost);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame(); 
            var currentLives = game.Lives;

            await game.ChangeDirection(Direction.Right);
            await _gameClock.Tick();

            game.Lives.Should().Be(currentLives - 1);
            game.Status.Should().Be(GameStatus.Dying);
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

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame(); 
            var currentLives = game.Lives;

            await game.ChangeDirection(Direction.Left);
            await _gameClock.Tick();
            await _gameClock.Tick();

            game.Lives.Should().Be(currentLives - 1);
        }

        [Fact]
        public async Task LivesDecreaseWhenCollidesWithGhostWhenPacManIsFacingAWall()
        {
            _gameSettings.Walls.Add(_gameSettings.PacMan.Location.Above);
            var ghost = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.Left)
                .WithChaseStrategyRight().Create();
            _gameSettings.Ghosts.Add(ghost);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame(); 
            var currentLives = game.Lives;

            await game.ChangeDirection(Direction.Up);
            await _gameClock.Tick();

            game.Lives.Should().Be(currentLives - 1);
        }

        [Fact]
        public async Task ShouldNotLoseLifeWhenAlreadyIsDying()
        {
            var expectedLife = 1;
            var location = new CellLocation(1, 1);

            _gameSettings.InitialGameStatus = GameStatus.Dying;
            _gameSettings.InitialLives = expectedLife;

            var ghost = GhostBuilder.New()
                .WithLocation(location)
                .Create();
            _gameSettings.Ghosts.Add(ghost);
            _gameSettings.PacMan = new PacMan(location, Direction.Down);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame(); 
            await _gameClock.Tick();

            game.Lives.Should().Be(expectedLife);
        }
    }
}
