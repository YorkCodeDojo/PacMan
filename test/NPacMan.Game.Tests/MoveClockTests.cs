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

        
        [Theory]
        [InlineData(1, 200, GhostStatus.Edible)]  //  50%
        [InlineData(1, 200, GhostStatus.Flash)]  //  50%
        [InlineData(4, 181, GhostStatus.Edible)]  //  55%
        [InlineData(4, 181, GhostStatus.Flash)]  //  55%
        [InlineData(5, 166, GhostStatus.Edible)]  //  60%
        [InlineData(5, 166, GhostStatus.Flash)]  //  60%
        public void GhostShouldTravelAtDifferentSpeedsForDifferentLevelsWhenFrightened(int level, int moveSpeedMilliseconds, GhostStatus ghostStatus)
        {
            var level2 = level;
            var moveClock = new MoveClock();
            var ghostName = "ghost1";
            moveClock.UpdateTime(TimeSpan.FromMinutes(1));
            if(!moveClock.ShouldGhostMove(level, ghostName, ghostStatus))
            {
                throw new Exception("PacMan Should always move on first move");
            }

            for (var j =0; j < 3; j++)
            {
                for (var i = 0; i < moveSpeedMilliseconds-1; i++)
                {
                    moveClock.UpdateTime(TimeSpan.FromMilliseconds(1));
                    if(moveClock.ShouldGhostMove(level, ghostName, ghostStatus))
                    {
                        throw new Exception($"PacMan moved early after just {i} milliseconds");
                    }
                }

                moveClock.UpdateTime(TimeSpan.FromMilliseconds(1));

                moveClock.ShouldGhostMove(level, ghostName, ghostStatus).Should().BeTrue();
                moveClock.ShouldGhostMove(level, ghostName, ghostStatus).Should().BeFalse();
            }
        }
    }
}