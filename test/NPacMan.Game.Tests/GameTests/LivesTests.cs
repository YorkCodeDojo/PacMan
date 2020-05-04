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
            var x = _gameSettings.PacMan.X + 1;
            var y = _gameSettings.PacMan.Y;

            _gameSettings.Ghosts.Add(new Ghost("Ghost1", x, y, CellLocation.TopLeft, new StandingStillGhostStrategy(), new StandingStillGhostStrategy()));

            var game = new Game(_gameClock, _gameSettings);
            var currentLives = game.Lives;

            game.ChangeDirection(Direction.Left);
            _gameClock.Tick();

            game.Lives.Should().Be(currentLives);
        }

        [Fact]
        public void LivesDecreaseByOneWhenCollidesWithGhost()
        {
            var x = _gameSettings.PacMan.X + 1;
            var y = _gameSettings.PacMan.Y;

            _gameSettings.Ghosts.Add(new Ghost("Ghost1", x, y, CellLocation.TopLeft, new StandingStillGhostStrategy(), new StandingStillGhostStrategy()));

            var game = new Game(_gameClock, _gameSettings);
            var currentLives = game.Lives;

            game.ChangeDirection(Direction.Right);
            _gameClock.Tick();

            game.Lives.Should().Be(currentLives - 1);
            game.PacMan.Status.Should().Be(PacManStatus.Dying);
        }


        [Fact]
        public void LivesDecreaseWhenCollidesWithGhostWalkingTowardsPacMan()
        {
            // G . . . P
            // . G . P .
            // . . PG . .

            _gameSettings.Ghosts.Add(new Ghost("Ghost1", _gameSettings.PacMan.X - 4, _gameSettings.PacMan.Y, CellLocation.TopLeft, new GhostGoesRightStrategy(), new StandingStillGhostStrategy()));

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
            var x = _gameSettings.PacMan.X;
            var y = _gameSettings.PacMan.Y;

            _gameSettings.Walls.Add((x, y - 1));
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", x - 1, y, CellLocation.TopLeft, new GhostGoesRightStrategy(), new StandingStillGhostStrategy()));

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
            var x = 1;
            var y = 1;

            _gameSettings.Ghosts.Add(new Ghost("Ghost1", x, y, CellLocation.TopLeft, new NPacMan.Game.StandingStillGhostStrategy(), new StandingStillGhostStrategy()));
            _gameSettings.PacMan = new PacMan(1, 1, Direction.Down, PacManStatus.Dying, expectedLife);

            var game = new Game(_gameClock, _gameSettings);
            _gameClock.Tick();

            game.Lives.Should().Be(expectedLife);
        }
    }
}
