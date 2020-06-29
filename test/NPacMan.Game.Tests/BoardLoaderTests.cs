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
                Doors = new CellLocation[] { new CellLocation(0, 5), new CellLocation(1, 5), new CellLocation(2, 5) },
                Width = 3,
                Height = 6,
                Portals = new Dictionary<CellLocation, CellLocation>
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
T*▲*T
 --- 
 HHH 
  F  ";
            var gameBoard = GameSettingsLoader.Load(board);

            gameBoard.Should().BeEquivalentTo(new
            {
                Walls = new CellLocation[] { (0, 0), (1, 0), (2, 0), (0, 1), (2, 1), (0, 2), (1, 2) },
                Coins = new CellLocation[] { (1, 1), (2, 2) },
                PowerPills = new CellLocation[] { (0, 3), (2, 3) },
                Doors = new CellLocation[] { new CellLocation(0, 4), new CellLocation(1, 4), new CellLocation(2, 4) },
                GhostHouse = new CellLocation[] { new CellLocation(0, 5), new CellLocation(1, 5), new CellLocation(2, 5) },
                Width = 3,
                Height = 7,
                Fruit = new CellLocation(1, 6),
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
 XFX ".Replace("?", expectedDirectionSymbol);

            var loadedBoard = GameSettingsLoader.Load(board);

            loadedBoard.PacMan.Should().BeEquivalentTo(new
            {
                Location = new CellLocation(2, 1),
                Direction = expectedDirection
            });
        }

        [Fact]
        public void ShouldHaveGhostsAtCorrectLocations()
        {
            var board = @" XXX 
     
 ▲XF
 ....
{""type"": ""Ghost"", ""name"": ""Pinky"", ""startingLocation"": {""x"":2,""y"":1}, ""scatterTarget"": {""x"":2, ""y"":3}, ""pillsToLeave"": 0 }
{""type"": ""Ghost"", ""name"": ""Blinky"", ""startingLocation"": {""x"":0,""y"":1}, ""scatterTarget"": {""x"":-10, ""y"":-3}, ""pillsToLeave"": 0 }
{""type"": ""Ghost"", ""name"": ""Clyde"", ""startingLocation"": {""x"":2,""y"":2}, ""scatterTarget"": {""x"":20, ""y"":33}, ""pillsToLeave"": 30 }
{""type"": ""Ghost"", ""name"": ""Inky"", ""startingLocation"": {""x"":1,""y"":1}, ""scatterTarget"": {""x"":1, ""y"":3}, ""pillsToLeave"": 90 }";

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
        public void ShouldHaveGhostsWithCorrectScatterTargetLocations()
        {
            var board = @" XXF 
 BIP 
 ▲XC 
 ....
{""type"": ""Ghost"", ""name"": ""Pinky"", ""startingLocation"": {""x"":16,""y"":14}, ""scatterTarget"": {""x"":2, ""y"":3}, ""pillsToLeave"": 0 }
{""type"": ""Ghost"", ""name"": ""Blinky"", ""startingLocation"": {""x"":13,""y"":11}, ""scatterTarget"": {""x"":-10, ""y"":-3}, ""pillsToLeave"": 0 }
{""type"": ""Ghost"", ""name"": ""Clyde"", ""startingLocation"": {""x"":14,""y"":14}, ""scatterTarget"": {""x"":20, ""y"":33}, ""pillsToLeave"": 30 }
{""type"": ""Ghost"", ""name"": ""Inky"", ""startingLocation"": {""x"":13,""y"":14}, ""scatterTarget"": {""x"":1, ""y"":3}, ""pillsToLeave"": 90 }";

            var loadedBoard = GameSettingsLoader.Load(board);
            loadedBoard.Ghosts.Should().BeEquivalentTo(
            new
            {
                ScatterStrategy = new {
                    DirectToLocation = new {
                        ScatterTarget = new { X = -10, Y = -3 }
                    }
                },
                Name = "Blinky",
            },
            new
            {
                ScatterStrategy = new {
                    DirectToLocation = new {
                        ScatterTarget = new { X = 1, Y = 3 }
                    }
                },
                Name = "Inky",
            },
            new
            {
                ScatterStrategy = new {
                    DirectToLocation = new {
                        ScatterTarget = new { X = 2, Y = 3 }
                    }
                },
                Name = "Pinky",
            },
            new
            {
                ScatterStrategy = new {
                    DirectToLocation = new {
                        ScatterTarget = new {  X = 20, Y = 33 }
                    }
                },
                Name = "Clyde",
            });
        }

    }
}
