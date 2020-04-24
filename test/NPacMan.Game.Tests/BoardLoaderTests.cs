using System;
using System.Collections.Generic;
using FluentAssertions;
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
P   P";
            var gameBoard = BoardLoader.Load(board);

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
        [InlineData("P")]
        [InlineData("PPP")]
        [InlineData("PPPP")]
        public void ShouldThrowIfIncorrectNumberOfPortals(string board)
        {
            Action action = () => BoardLoader.Load(board);

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

            var loadedBoard = BoardLoader.Load(board);

            loadedBoard.PacMan.Should().BeEquivalentTo(new
            {
                X = 2,
                Y = 1,
                Direction = expectedDirection
            });
        }

    }

}
