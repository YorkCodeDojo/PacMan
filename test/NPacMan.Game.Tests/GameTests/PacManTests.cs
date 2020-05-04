﻿using System;
using FluentAssertions;
using Xunit;

namespace NPacMan.Game.Tests.GameTests
{
    public class PacManTests
    {
        private readonly TestGameSettings _gameSettings;
        private readonly TestGameClock _gameClock;

        public PacManTests()
        {
            _gameSettings = new TestGameSettings();
            _gameClock = new TestGameClock();
        }

        [Fact]
        public void PacManStartsInInitialPosition()
        {
            _gameSettings.PacMan = new PacMan(5, 6, Direction.Right, PacManStatus.Alive, 3);
            var game = new Game(_gameClock, _gameSettings);

            game.PacMan.Should().BeEquivalentTo(new
            {
                X = 5,
                Y = 6,
                Direction = Direction.Right
            });
        }


        [Theory]
        [InlineData(Direction.Up, 0, -1)]
        [InlineData(Direction.Down, 0, +1)]
        [InlineData(Direction.Left, -1, 0)]
        [InlineData(Direction.Right, +1, 0)]
        public void PacManWalksInFacingDirection(Direction directionToFace, int changeInX, int changeInY)
        {
            var game = new Game(_gameClock, _gameSettings);
            var x = game.PacMan.X;
            var y = game.PacMan.Y;

            game.ChangeDirection(directionToFace);

            _gameClock.Tick();

            game.PacMan.Should().BeEquivalentTo(new
            {
                X = x + changeInX,
                Y = y + changeInY,
                Direction = directionToFace
            });
        }

        [Theory]
        [InlineData(Direction.Up, 0, -1)]
        [InlineData(Direction.Down, 0, +1)]
        [InlineData(Direction.Left, -1, 0)]
        [InlineData(Direction.Right, +1, 0)]
        public void PacManCannotMoveIntoWalls(Direction directionToFace, int createWallXOffset, int createWallYOffset)
        {
            var game = new Game(_gameClock, _gameSettings);
            var x = game.PacMan.X;
            var y = game.PacMan.Y;
            var score = game.Score;

            game.ChangeDirection(directionToFace);

            _gameSettings.Walls.Add((x + createWallXOffset, y + createWallYOffset));

            _gameClock.Tick();

            game.PacMan.Should().BeEquivalentTo(new
            {
                X = x,
                Y = y,
                Direction = directionToFace
            });

            game.Score.Should().Be(score);
        }

        [Fact]
        public void PacManIsTeleportedWhenYouWalkIntoAPortal()
        {
            var game = new Game(_gameClock, _gameSettings);
            var x = game.PacMan.X;
            var y = game.PacMan.Y;
            var score = game.Score;

            _gameSettings.Portals.Add((x - 1, y), (15, 15));

            game.ChangeDirection(Direction.Left);

            _gameClock.Tick();

            game.PacMan.Should().BeEquivalentTo(new
            {
                X = 14,
                Y = 15,
                Direction = Direction.Left
            });

            game.Score.Should().Be(score);
        }

        [Theory]
        [InlineData(PacManStatus.Dead)]
        [InlineData(PacManStatus.Dying)]
        [InlineData(PacManStatus.Respawning)]
        public void PacManShouldNotMoveInCertainStates(PacManStatus state)
        {
            var x = 1;
            var y = 1;

            _gameSettings.PacMan = new PacMan(x, y, Direction.Down, state, 1);

            var game = new Game(_gameClock, _gameSettings);
            _gameClock.Tick();

            game.PacMan
                .Should().BeEquivalentTo(new
                {
                    X = x,
                    Y = y
                });
        }

        [Fact]
        public void PacManShouldRespawnAfter4Seconds()
        {
            _gameSettings.PacMan = new PacMan(1, 1, Direction.Down, PacManStatus.Alive, 1);
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", 1, 2, new NPacMan.Game.StandingStillGhostStrategy(), new StandingStillGhostStrategy()));

            var game = new Game(_gameClock, _gameSettings);
            var now = DateTime.UtcNow;
            _gameClock.Tick(now);

            _gameClock.Tick(now.AddSeconds(1));
            _gameClock.Tick(now.AddSeconds(2));
            _gameClock.Tick(now.AddSeconds(3));

            if (game.PacMan.Status != PacManStatus.Dying)
                throw new Exception($"Invalid PacMan State {game.PacMan.Status:G}");

            _gameClock.Tick(now.AddSeconds(4));

            game.PacMan.Status.Should().Be(PacManStatus.Respawning);
        }
    }
}