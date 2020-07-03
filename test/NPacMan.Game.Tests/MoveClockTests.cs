using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace NPacMan.Game.Tests
{
    public class MoveClockTests
    {
        private static class PercentagesInMilliseconds
        {
            // Percent => Milliseconds : m = 100 / (p / 100)
            public const int Percent50 = 200;
            public const int Percent55 = 181;
            public const int Percent60 = 166;
            public const int Percent75 = 133;
            public const int Percent80 = 125;
            public const int Percent85 = 117;
            public const int Percent90 = 111;
            public const int Percent95 = 105;
            public const int Percent100 = 100;
            public const int Percent160 = 62;
        }


        [Theory]
        [InlineData(1, PercentagesInMilliseconds.Percent80)]  // 80%
        [InlineData(2, PercentagesInMilliseconds.Percent90)]  // 90%
        [InlineData(5, PercentagesInMilliseconds.Percent100)]  // 100%
        [InlineData(21, PercentagesInMilliseconds.Percent90)]  // 90%
        public void PacManShouldTravelAt80PercentAtLevelZero(int level, int moveSpeedMilliseconds)
        {
            TestMoveClock(moveSpeedMilliseconds,
                   moveClock => moveClock.ShouldPacManMove(level, false));
        }

        [Theory]
        [InlineData(1, PercentagesInMilliseconds.Percent90)]  // 90%
        [InlineData(2, PercentagesInMilliseconds.Percent95)]  // 95%
        [InlineData(5, PercentagesInMilliseconds.Percent100)]  // 100%
        [InlineData(21, PercentagesInMilliseconds.Percent95)]  // 95%
        public void PacManShouldTravelAtDifferentSpeedsForDifferentLevelsWhenFrightened(int level, int moveSpeedMilliseconds)
        {
            TestMoveClock(moveSpeedMilliseconds,
                   moveClock => moveClock.ShouldPacManMove(level, true) );     
        }

        [Theory]
        [InlineData(1, PercentagesInMilliseconds.Percent50, GhostStatus.Edible)]  //  50%
        [InlineData(1, PercentagesInMilliseconds.Percent50, GhostStatus.Flash)]  //  50%
        [InlineData(4, PercentagesInMilliseconds.Percent55, GhostStatus.Edible)]  //  55%
        [InlineData(4, PercentagesInMilliseconds.Percent55, GhostStatus.Flash)]  //  55%
        [InlineData(5, PercentagesInMilliseconds.Percent60, GhostStatus.Edible)]  //  60%
        [InlineData(5, PercentagesInMilliseconds.Percent60, GhostStatus.Flash)]  //  60%
        public void GhostShouldTravelAtDifferentSpeedsForDifferentLevelsWhenFrightened(int level, int moveSpeedMilliseconds, GhostStatus ghostStatus)
        {
            var ghostName = Guid.NewGuid().ToString();
            TestMoveClock(moveSpeedMilliseconds,
                   moveClock => moveClock.ShouldGhostMove(level, ghostName, ghostStatus) );            
        }

        [Theory]
        [InlineData(1, PercentagesInMilliseconds.Percent75)]  //  75%
        [InlineData(4, PercentagesInMilliseconds.Percent85)]  //  85%
        [InlineData(5, PercentagesInMilliseconds.Percent95)]  //  95%
        public void GhostShouldTravelAtDifferentSpeedsForDifferentLevelsWhenAlive(int level, int moveSpeedMilliseconds)
        {
            var ghostName = Guid.NewGuid().ToString();
            TestMoveClock(moveSpeedMilliseconds,
                   moveClock => moveClock.ShouldGhostMove(level, ghostName, GhostStatus.Alive) );
        }

        [Theory]
        [InlineData(1, PercentagesInMilliseconds.Percent160)]
        [InlineData(4, PercentagesInMilliseconds.Percent160)]
        [InlineData(5, PercentagesInMilliseconds.Percent160)]
        public void GhostShouldTravelAtDifferentSpeedsForDifferentLevelsWhenRunningHome(int level, int moveSpeedMilliseconds)
        {
            var ghostName = Guid.NewGuid().ToString();
            TestMoveClock(moveSpeedMilliseconds,
                   moveClock => moveClock.ShouldGhostMove(level, ghostName, GhostStatus.RunningHome) );
        }

        private void TestMoveClock(int moveSpeedMilliseconds, Func<MoveClock, bool> action)
        {
            var moveClock = new MoveClock();
            moveClock.UpdateTime(TimeSpan.FromMinutes(1));
            if(!action(moveClock))
            {
                throw new Exception("Game piece should always move on first move");
            }

            for (var j =0; j < 3; j++)
            {
                for (var i = 0; i < moveSpeedMilliseconds-1; i++)
                {
                    moveClock.UpdateTime(TimeSpan.FromMilliseconds(1));
                    if(action(moveClock))
                    {
                        throw new Exception($"Game piece moved early after just {i} milliseconds");
                    }
                }

                moveClock.UpdateTime(TimeSpan.FromMilliseconds(1));

                action(moveClock).Should().BeTrue();
                action(moveClock).Should().BeFalse();
            }
        }

    }
}