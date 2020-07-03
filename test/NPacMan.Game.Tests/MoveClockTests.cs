using FluentAssertions;
using System;
using Xunit;

namespace NPacMan.Game.Tests
{
    public class MoveClockTests
    {
        public const int MaxLevel = 25;
        public const int MaxCoins = 150;

        [Theory]
        [ClassData(typeof(PacMPacManShouldTravelAtDifferentSpeedsForDifferentLevelsWhenNotFrightenedTestData))]
        public void PacManShouldTravelAtDifferentSpeedsForDifferentLevelsWhenNotFrightened(int level, int moveSpeedMilliseconds)
        {
            TestMoveClock(moveSpeedMilliseconds,
                   moveClock => moveClock.ShouldPacManMove(level, false));
        }

        [Theory]
        [ClassData(typeof(PacManShouldTravelAtDifferentSpeedsForDifferentLevelsWhenFrightenedTestData))]
        public void PacManShouldTravelAtDifferentSpeedsForDifferentLevelsWhenFrightened(int level, int moveSpeedMilliseconds)
        {
            TestMoveClock(moveSpeedMilliseconds,
                   moveClock => moveClock.ShouldPacManMove(level, true));
        }

        [Theory]
        [ClassData(typeof(GhostShouldTravelAtDifferentSpeedsForDifferentLevelsWhenFrightenedTestData))]
        public void GhostShouldTravelAtDifferentSpeedsForDifferentLevelsWhenFrightened(int level, int moveSpeedMilliseconds, GhostStatus ghostStatus)
        {
            var ghostName = Guid.NewGuid().ToString();

            TestMoveClock(moveSpeedMilliseconds,
                   moveClock => moveClock.ShouldGhostMove(level, coinsRemaining: int.MaxValue, ghostName, ghostStatus, false));
        }

        [Theory]
        [ClassData(typeof(GhostShouldTravelAtDifferentSpeedsForDifferentLevelsWhenAliveTestData))]
        public void GhostShouldTravelAtDifferentSpeedsForDifferentLevelsWhenAlive(int level, int moveSpeedMilliseconds)
        {
            var ghostName = Guid.NewGuid().ToString();

            TestMoveClock(moveSpeedMilliseconds,
                   moveClock => moveClock.ShouldGhostMove(level, coinsRemaining: int.MaxValue, ghostName, GhostStatus.Alive, false));
        }

        [Theory]
        [ClassData(typeof(GhostShouldTravelAtDifferentSpeedsForDifferentLevelsWhenRunningHomeTestData))]
        public void GhostShouldTravelAtDifferentSpeedsForDifferentLevelsWhenRunningHome(int level, int moveSpeedMilliseconds)
        {
            var ghostName = Guid.NewGuid().ToString();

            TestMoveClock(moveSpeedMilliseconds,
                   moveClock => moveClock.ShouldGhostMove(level, coinsRemaining: int.MaxValue, ghostName, GhostStatus.RunningHome, false));
        }

        [Theory]
        [ClassData(typeof(BlinkyShouldTravelAtDifferentSpeedsDuringCruiseElroyTestData))]
        public void BlinkyShouldTravelAtDifferentSpeedsDuringCruiseElroy(int level, int coinsLeft, int moveSpeedMilliseconds)
        {
            var ghostName = GhostNames.Blinky;
            TestMoveClock(moveSpeedMilliseconds,
                   moveClock => moveClock.ShouldGhostMove(level, coinsLeft, ghostName, GhostStatus.Alive, false));
        }

        [Theory]
        [ClassData(typeof(GhostShouldTravelAtDifferentSpeedsWhenInTheTunnelTestData))]
        public void GhostShouldTravelAtDifferentSpeedsWhenInTheTunnel(int level, int moveSpeedMilliseconds, GhostStatus ghostStatus)
        {
            var ghostName = Guid.NewGuid().ToString();

            TestMoveClock(moveSpeedMilliseconds,
                   moveClock => moveClock.ShouldGhostMove(level, coinsRemaining: int.MaxValue, ghostName, ghostStatus,inTunnel: true));
        }

        private void TestMoveClock(int moveSpeedMilliseconds, Func<MoveClock, bool> action)
        {
            var moveClock = new MoveClock();
            moveClock.UpdateTime(TimeSpan.FromMinutes(1));
            if (!action(moveClock))
            {
                throw new Exception("Game piece should always move on first move");
            }

            for (var j = 0; j < 3; j++)
            {
                for (var i = 0; i < moveSpeedMilliseconds - 1; i++)
                {
                    moveClock.UpdateTime(TimeSpan.FromMilliseconds(1));
                    if (action(moveClock))
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