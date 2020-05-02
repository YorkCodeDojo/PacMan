using FluentAssertions;
using Xunit;

namespace NPacMan.Game.Tests.GameTests
{
    public class GameTestsxx
    {
        private readonly TestGameSettings _gameSettings;
        private readonly Game _game;

        public GameTestsxx()
        {
            _gameSettings = new TestGameSettings();
            var gameClock = new TestGameClock();
            _game = new Game(gameClock, _gameSettings);
        }

        [Fact]
        public void GameStartsWithThreeLives()
        {
            _game.Lives.Should().Be(3);
        }

        [Fact]
        public void TheGameCanReadTheWidthFromTheBoard()
        {
            var gameBoardWidth = 100;
            _gameSettings.Width = gameBoardWidth;

            _game.Width.Should().Be(gameBoardWidth);
        }

        [Fact]
        public void TheGameCanReadTheHeightFromTheBoard()
        {
            var gameBoardHeight = 100;
            _gameSettings.Height = gameBoardHeight;

            _game.Height.Should().Be(gameBoardHeight);
        }
    }
}
