using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NPacMan.Game.Tests
{
    public class TestGameClock : IGameClock
    {
        private List<Action> _actions = new List<Action>();

        public void Subscribe(Action action)
        {
            _actions.Add(action);
        }

        public void Tick()
        {
            _actions.ForEach(x => x());
        }
    }
    public class GameTests
    {
        // 1. Walks in facing direction
        // 2. Does not walk when wall in the way
        // 3. Can't turn to face a wall
        // 4. Increments score by 10 when a coin when collected.
        // 5. Coin is removed from game when collected.
        // 6. Game ends when all coins are collected.

        [Theory]
        [InlineData(Direction.Up, 0, -1)]
        [InlineData(Direction.Down, 0, +1)]
        [InlineData(Direction.Left, -1, 0)]
        [InlineData(Direction.Right, +1, 0)]
        public void WalksInFacingDirection(Direction directionToFace,  int changeInX, int changeInY)
        {
            var gameClock = new TestGameClock();
            var game = new Game(gameClock);
            
            var x = game.PacMan.X;
            var y = game.PacMan.Y;
            
            game.ChangeDirection(directionToFace);

            gameClock.Tick();

            game.PacMan.Should().BeEquivalentTo(new {
                X = x + changeInX,
                Y = y + changeInY,
                Direction = directionToFace
            });
        }
    }
}
