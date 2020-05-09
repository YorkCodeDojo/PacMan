using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace NPacMan.Game.Tests
{
    public class BoardLoaderTests
    {
        [Fact]
        public void TheBoardCanBeLoadedFromATextFile()
        {
            var filename = "SimpleTestBoard.txt";

            var gameBoard = GameSettingsLoader.LoadFromFile(filename);

            gameBoard.Should().BeEquivalentTo(new
            {
                Walls = new CellLocation[] { (0, 0), (1, 0), (2, 0), (0, 1), (2, 1), (0, 2), (1, 2) },
                Coins = new CellLocation[] { (1, 1), (2, 2) },
                Doors = new CellLocation[] { new CellLocation(0, 4), new CellLocation(1, 4), new CellLocation(2, 4) },
                Width = 3,
                Height = 5,
                Portals = new Dictionary<CellLocation, CellLocation >
                {
                    {(-1,3), (3,3) },{(3,3), (-1,3) }
                }
            });

        }

        [Fact]
        public void ShouldItemsOnBoard()
        {
            var board = @" XXX 
 X.X 
 XX. 
T ▲ T
 --- ";
            var gameBoard = GameSettingsLoader.Load(board);

            gameBoard.Should().BeEquivalentTo(new
            {
                Walls = new CellLocation[] { (0, 0), (1, 0), (2, 0), (0, 1), (2, 1), (0, 2), (1, 2) },
                Coins = new CellLocation[] { (1, 1), (2, 2) },
                Doors = new CellLocation[] { new CellLocation(0, 4), new CellLocation(1, 4), new CellLocation(2, 4) },
                Width = 3,
                Height = 5,
                Portals = new Dictionary<CellLocation, CellLocation>
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
                Location = new CellLocation(2,1),
                Direction = expectedDirection
            });
        }

        [Fact]
        public void ShouldHaveGhostsAtCorrectLocations()
        {
            var board = @" XXX 
 BIP 
 ▲XC
 ....
{Blinky=-10,-3}
{Inky=1,3}
{Pinky=2,3}
{Clyde=20,33}";

            var loadedBoard = GameSettingsLoader.Load(board);

            loadedBoard.Ghosts.Should().BeEquivalentTo(
            new
            {
                Location = new
                {
                    X = 0,
                    Y = 1
                },
                Name = "Blinky",
            },
            new
            {
                Location = new
                {
                    X = 1,
                    Y = 1
                },
                Name = "Inky",
            },
            new
            {
                Location = new
                {
                    X = 2,
                    Y = 1
                },
                Name = "Pinky",
            },
            new
            {
                Location = new
                {
                    X = 2,
                    Y = 2
                },
                Name = "Clyde",
            });
        }

        [Fact]
        public void ShouldHaveGhostsWithCorrectHomeLocations()
        {
            var board = @" XXX 
 BIP 
 ▲XC 
 ....
{Blinky=-10,-3}
{Inky=1,3}
{Pinky=2,3}
{Clyde=20,33}";

            var loadedBoard = GameSettingsLoader.Load(board);
            loadedBoard.Ghosts.Should().BeEquivalentTo(
            new
            {
                ScatterTarget = new { X = -10, Y = -3 },
                Name = "Blinky",
            },
            new
            {
                ScatterTarget = new { X = 1, Y = 3 },
                Name = "Inky",
            },
            new
            {
                ScatterTarget = new { X = 2, Y = 3 },
                Name = "Pinky",
            },
            new
            {
                ScatterTarget = new { X = 20, Y = 33 },
                Name = "Clyde",
            });
        }

    }
}
