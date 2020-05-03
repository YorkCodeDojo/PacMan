using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace NPacMan.Game.Tests
{
    public class BoardLoaderTests
    {
        [Fact]
        public void ShouldItemsOnBoard()
        {
            var board = @" XXX 
 X.X 
 XX. 
T ▲ T";
            var gameBoard = GameSettingsLoader.Load(board);

            gameBoard.Should().BeEquivalentTo(new
            {
                Walls = new[] { (0, 0), (1, 0), (2, 0), (0, 1), (2, 1), (0, 2), (1, 2) },
                Coins = new[] { (1, 1), (2, 2) },
                Width = 3,
                Height = 4,
                Portals = new Dictionary<(int, int), (int, int)>
                {
                    {(-1,3), (3,3) },{(3,3), (-1,3) }
                }
            });

        }

        [Theory]
        [InlineData("T")]
        [InlineData("TTT")]
        [InlineData("TTTT")]
        public void ShouldThrowIfIncorrectNumberOfPortals(string board)
        {
            Action action = () => GameSettingsLoader.Load(board);

            action.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData("▲", Direction.Up)]
        [InlineData("▼", Direction.Down)]
        [InlineData("►", Direction.Right)]
        [InlineData("◄", Direction.Left)]
        public void ShouldHavePacManAtLocationAndDirection(string expectedDirectionSymbol, Direction expectedDirection)
        {
            var board = @" XXX 
 XX? 
 XXX ".Replace("?", expectedDirectionSymbol);

            var loadedBoard = GameSettingsLoader.Load(board);

            loadedBoard.PacMan.Should().BeEquivalentTo(new
            {
                X = 2,
                Y = 1,
                Direction = expectedDirection
            });
        }

        [Fact]
        public void ShouldHaveGhostsAtCorrectLocations()
        {
            var board = @" XXX 
 BIP 
 ▲XC
 bipc";

            var loadedBoard = GameSettingsLoader.Load(board);

            loadedBoard.Ghosts.Should().BeEquivalentTo(
            new
            {
                X = 0,
                Y = 1,
                Name = "Blinky",
            },
            new
            {
                X = 1,
                Y = 1,
                Name = "Inky",
            },
            new
            {
                X = 2,
                Y = 1,
                Name = "Pinky",
            },
            new
            {
                X = 2,
                Y = 2,
                Name = "Clyde",
            });
        }

        [Fact]
        public void ShouldHaveGhostsWithCorrectHomeLocations()
        {
            var board = @" XXX 
 BIP 
 ▲XC 
 bipc";

            var loadedBoard = GameSettingsLoader.Load(board);

            using var _ = new AssertionScope();
            var blinky = loadedBoard.Ghosts.Single(ghost => ghost.Name == "Blinky");
            ((MoveHomeGhostStrategy)blinky.HomeStrategy).X.Should().Be(0);
            ((MoveHomeGhostStrategy)blinky.HomeStrategy).Y.Should().Be(3);

            var inky = loadedBoard.Ghosts.Single(ghost => ghost.Name == "Inky");
            ((MoveHomeGhostStrategy)inky.HomeStrategy).X.Should().Be(1);
            ((MoveHomeGhostStrategy)inky.HomeStrategy).Y.Should().Be(3);

            var pinky = loadedBoard.Ghosts.Single(ghost => ghost.Name == "Pinky");
            ((MoveHomeGhostStrategy)pinky.HomeStrategy).X.Should().Be(2);
            ((MoveHomeGhostStrategy)pinky.HomeStrategy).Y.Should().Be(3);

            var clyde = loadedBoard.Ghosts.Single(ghost => ghost.Name == "Clyde");
            ((MoveHomeGhostStrategy)clyde.HomeStrategy).X.Should().Be(3);
            ((MoveHomeGhostStrategy)clyde.HomeStrategy).Y.Should().Be(3);
        }

        [Fact]
        public void AGhostsHomeLocationIsAlsoACoin()
        {
            var board = @" XXX 
 BIP 
 ▲XC 
 bipc";
            var loadedBoard = GameSettingsLoader.Load(board);

            loadedBoard.Coins.Should().BeEquivalentTo( (0,3), (1, 3), (2, 3), (3, 3) );
        }

    }
}
