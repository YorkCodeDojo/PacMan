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
        public void LivesStayTheSameWhenNotCollidingWithAGhost()
        {
            var x = _gameSettings.PacMan.Location.X + 1;
            var y = _gameSettings.PacMan.Location.Y;

            _gameSettings.Ghosts.Add(new Ghost("Ghost1", new CellLocation(x, y), Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));

            var game = new Game(_gameClock, _gameSettings);
            var currentLives = game.Lives;

            game.ChangeDirection(Direction.Left);
            _gameClock.Tick();

            game.Lives.Should().Be(currentLives);
        }

        [Fact]
        public void LivesDecreaseByOneWhenCollidesWithGhost()
        {
            var x = _gameSettings.PacMan.Location.X + 1;
            var y = _gameSettings.PacMan.Location.Y;

            _gameSettings.Ghosts.Add(new Ghost("Ghost1", new CellLocation(x, y), Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));

            var game = new Game(_gameClock, _gameSettings);
            var currentLives = game.Lives;

            game.ChangeDirection(Direction.Right);
            _gameClock.Tick();

            game.Lives.Should().Be(currentLives - 1);
            game.Status.Should().Be(GameStatus.Dying);
        }


        [Fact]
        public void LivesDecreaseWhenCollidesWithGhostWalkingTowardsPacMan()
        {
            // G . . . P
            // . G . P .
            // . . PG . .
            var ghostLocation = new CellLocation(_gameSettings.PacMan.Location.X - 4, _gameSettings.PacMan.Location.Y);
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", ghostLocation, Direction.Left, CellLocation.TopLeft, new GhostGoesRightStrategy()));

            var game = new Game(_gameClock, _gameSettings);
            var currentLives = game.Lives;

            game.ChangeDirection(Direction.Left);
            _gameClock.Tick();
            _gameClock.Tick();

            game.Lives.Should().Be(currentLives - 1);
        }

        [Fact]
        public void LivesDecreaseWhenCollidesWithGhostWhenPacManIsFacingAWall()
        {
            var x = _gameSettings.PacMan.Location.X;
            var y = _gameSettings.PacMan.Location.Y;

            _gameSettings.Walls.Add((x, y - 1));
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", new CellLocation(x - 1, y), Direction.Right, CellLocation.TopLeft, new GhostGoesRightStrategy()));

            var game = new Game(_gameClock, _gameSettings);
            var currentLives = game.Lives;

            game.ChangeDirection(Direction.Up);
            _gameClock.Tick();

            game.Lives.Should().Be(currentLives - 1);
        }

        [Fact]
        public void ShouldNotLoseLifeWhenAlreadyIsDying()
        {
            var expectedLife = 1;
            var location = new CellLocation(1, 1);

            _gameSettings.InitialGameStatus = GameStatus.Dying;
            _gameSettings.InitialLives = expectedLife;

            _gameSettings.Ghosts.Add(new Ghost("Ghost1", location, Direction.Left, CellLocation.TopLeft, new StandingStillGhostStrategy()));
            _gameSettings.PacMan = new PacMan(location, Direction.Down);

            var game = new Game(_gameClock, _gameSettings);
            _gameClock.Tick();

            game.Lives.Should().Be(expectedLife);
        }
    }
}
