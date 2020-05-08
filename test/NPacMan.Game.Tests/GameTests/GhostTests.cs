﻿using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace NPacMan.Game.Tests.GameTests
{
    public class GhostTests
    {
        private readonly TestGameSettings _gameSettings;
        private readonly TestGameClock _gameClock;

        public GhostTests()
        {
            _gameSettings = new TestGameSettings();
            _gameClock = new TestGameClock();
        }

        [Fact]
        public void GhostMovesInDirectionOfStrategy()
        {
            var strategy = new GhostGoesRightStrategy();
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", new CellLocation(0, 0), Direction.Left, CellLocation.TopLeft, strategy));

            var game = new Game(_gameClock, _gameSettings);

            _gameClock.Tick();
            game.Ghosts["Ghost1"].Should().BeEquivalentTo(new
            {
                Location = new {
                    X = 1,
                    Y = 0
                }
            });
        }


        [Fact]
        public void GhostShouldNotMoveWhenPacManIsDying()
        {
            var x = 1;
            var y = 1;

            _gameSettings.Ghosts.Add(new Ghost("Ghost1", new CellLocation(x, y), Direction.Left, CellLocation.TopLeft, new DirectToStrategy(new DirectToPacManLocation())));
            _gameSettings.PacMan = new PacMan((3, 3), Direction.Down, PacManStatus.Dying, 1);

            var game = new Game(_gameClock, _gameSettings);
            _gameClock.Tick();

            using var _ = new AssertionScope();
            game.Ghosts.Values.First()
                .Should().BeEquivalentTo(new
                {
                    Location = new {
                        X = x,
                        Y = y
                    }
                });
        }



        [Fact]
        public void GhostShouldBeHiddenWhenPacManIsReSpawning()
        {
            _gameSettings.PacMan = new PacMan((1, 1), Direction.Left, PacManStatus.Respawning, 1);
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", new CellLocation(1, 2), Direction.Left, CellLocation.TopLeft, new GhostGoesRightStrategy()));

            var game = new Game(_gameClock, _gameSettings);

            game.Ghosts.Should().BeEmpty();
        }

        [Fact]
        public void GhostShouldBeBackAtHomeAfterPacManDiesAndComesBackToLife()
        {
            _gameSettings.PacMan = new PacMan((3, 2), Direction.Left, PacManStatus.Alive, 2);
            var homeLocation = new CellLocation(1, 2);
            var strategy = new GhostGoesRightStrategy();
            _gameSettings.Ghosts.Add(new Ghost("Ghost1", homeLocation, Direction.Right, CellLocation.TopLeft, strategy));

            var game = new Game(_gameClock, _gameSettings);
            var now = DateTime.UtcNow;

            _gameClock.Tick(now);
            _gameClock.Tick(now.AddSeconds(4));

            if (game.PacMan.Status != PacManStatus.Respawning)
                throw new Exception($"Invalid PacMan State {game.PacMan.Status:G} Should be Respawning");

            _gameClock.Tick(now.AddSeconds(8));
            
            if (game.PacMan.Status != PacManStatus.Alive)
                throw new Exception($"Invalid PacMan State {game.PacMan.Status:G} Should be Alive");
            
            game.Ghosts.Values.First()
                .Should().BeEquivalentTo(new
                {
                    Location = new {
                        X = homeLocation.X,
                        Y = homeLocation.Y
                    }
                });
        }
    }
}
