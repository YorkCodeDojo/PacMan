using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using static NPacMan.Game.Tests.GameTests.Ensure;

namespace NPacMan.Game.Tests.GameTests
{
    public class GameTests
    {
        private readonly TestGameSettings _gameSettings;
        private readonly Game _game;

        public GameTests()
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

        [Fact]
        public void CanReadPortalsFromGame(){
            _gameSettings.Portals.Add((1,2),(3,4));

            _game.Portals.Should().BeEquivalentTo(new Dictionary<CellLocation, CellLocation> {
                [(1,2)] = (3,4)
            });
        }

    }
}
