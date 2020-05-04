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
            loadedBoard.Ghosts.Should().BeEquivalentTo(
            new
            {
                HomeLocationX = 0,
                HomeLocationY = 3,
                Name = "Blinky",
            },
            new
            {
                HomeLocationX = 1,
                HomeLocationY = 3,
                Name = "Inky",
            },
            new
            {
                HomeLocationX = 2,
                HomeLocationY = 3,
                Name = "Pinky",
            },
            new
            {
                HomeLocationX = 3,
                HomeLocationY = 3,
                Name = "Clyde",
            });
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
