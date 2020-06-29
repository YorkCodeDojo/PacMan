using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace NPacMan.Game.Tests
{
    public class MoveClockTests
    {
        [Theory]
        [InlineData(1, 125)]  // 80%
        [InlineData(2, 111)]  // 90%
        [InlineData(5, 100)]  // 100%
        [InlineData(21, 111)]  // 90%
        public void PacManShouldTravelAt80PercentAtLevelZero(int level, int moveSpeedMilliseconds)
        {
            var moveClock = new MoveClock();
            
            moveClock.UpdateTime(TimeSpan.FromMinutes(1));
            if(!moveClock.ShouldPacManMove(level, false))
            {
                throw new Exception("PacMan Should always move on first move");
            }

            for (var j =0; j < 3; j++)
            {
                for (var i = 0; i < moveSpeedMilliseconds-1; i++)
                {
                    moveClock.UpdateTime(TimeSpan.FromMilliseconds(1));
                    if(moveClock.ShouldPacManMove(level, false))
                    {
                        throw new Exception($"PacMan moved early after just {i} milliseconds");
                    }
                }

                moveClock.UpdateTime(TimeSpan.FromMilliseconds(1));

                moveClock.ShouldPacManMove(level, false).Should().BeTrue();
                moveClock.ShouldPacManMove(level, false).Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(1, 111)]  // 90%
        [InlineData(2, 105)]  // 95%
        [InlineData(5, 100)]  // 100%
        [InlineData(21, 105)]  // 95%
        public void PacManShouldTravelAtDifferentSpeedsForDifferentLevelsWhenFrightened(int level, int moveSpeedMilliseconds)
        {
            var moveClock = new MoveClock();
            
            moveClock.UpdateTime(TimeSpan.FromMinutes(1));
            if(!moveClock.ShouldPacManMove(level, true))
            {
                throw new Exception("PacMan Should always move on first move");
            }

            for (var j =0; j < 3; j++)
            {
                for (var i = 0; i < moveSpeedMilliseconds-1; i++)
                {
                    moveClock.UpdateTime(TimeSpan.FromMilliseconds(1));
                    if(moveClock.ShouldPacManMove(level, isFrightened: true))
                    {
                        throw new Exception($"PacMan moved early after just {i} milliseconds");
                    }
                }

                moveClock.UpdateTime(TimeSpan.FromMilliseconds(1));

                moveClock.ShouldPacManMove(level, true).Should().BeTrue();
                moveClock.ShouldPacManMove(level, true).Should().BeFalse();
            }
        }
    }
}