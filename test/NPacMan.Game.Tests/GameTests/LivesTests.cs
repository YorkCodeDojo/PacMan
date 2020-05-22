using System.Threading.Tasks;
using FluentAssertions;
using NPacMan.Game.Tests.GhostStrategiesForTests;
using Xunit;
using static NPacMan.Game.Tests.GameTests.Ensure;

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
            var x = _gameSettings.PacMan.Location.X + 1;
            var y = _gameSettings.PacMan.Location.Y;

            _gameSettings.Ghosts.Add(new Ghost("Ghost1", new CellLocation(x, y), Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));

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
            var x = _gameSettings.PacMan.Location.X + 1;
            var y = _gameSettings.PacMan.Location.Y;

            _gameSettings.Ghosts.Add(new Ghost("Ghost1", new CellLocation(x, y), Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));

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
            var ghostLocation = new CellLocation(_gameSettings.PacMan.Location.X - 4, _gameSettings.PacMan.Location.Y);
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", ghostLocation, Direction.Left, CellLocation.TopLeft, new GhostGoesRightStrategy()));

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
            var x = _gameSettings.PacMan.Location.X;
            var y = _gameSettings.PacMan.Location.Y;

            _gameSettings.Walls.Add((x, y - 1));
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", new CellLocation(x - 1, y), Direction.Right, CellLocation.TopLeft, new GhostGoesRightStrategy()));

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

            _gameSettings.Ghosts.Add(new Ghost("Ghost1", location, Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));
            _gameSettings.PacMan = new PacMan(location, Direction.Down);

            var game = new Game(_gameClock, _gameSettings);
            game.StartGame(); 
            await _gameClock.Tick();

            game.Lives.Should().Be(expectedLife);
        }
    }
}
