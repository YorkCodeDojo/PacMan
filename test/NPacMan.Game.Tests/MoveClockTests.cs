using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace NPacMan.Game.Tests
{
    public class PacMPacManShouldTravelAtDifferentSpeedsForDifferentLevelsWhenNotFrightenedTestData : IEnumerable<object[]>
    {
        private object[] CreateTestData(int level, int milliseconds)
        {
            return new object[] { level, milliseconds };
        }
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return CreateTestData(1, PercentagesInMilliseconds.Percent80);

            for (var level = 2; level <= 4; level++)
            {
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent90);
            }

            for (var level = 5; level <= 20; level++)
            {
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent100);
            }

            for (var level = 21; level <= 256; level++)
            {
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent90);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class PacManShouldTravelAtDifferentSpeedsForDifferentLevelsWhenFrightenedTestData : IEnumerable<object[]>
    {
        private object[] CreateTestData(int level, int milliseconds)
        {
            return new object[] { level, milliseconds };
        }
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return CreateTestData(1, PercentagesInMilliseconds.Percent90);

            for (var level = 2; level <= 4; level++)
            {
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent95);
            }

            for (var level = 5; level <= 20; level++)
            {
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent100);
            }

            for (var level = 21; level <= 256; level++)
            {
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent95);
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class GhostShouldTravelAtDifferentSpeedsForDifferentLevelsWhenFrightenedTestData : IEnumerable<object[]>
    {
        private object[] CreateTestData(int level, int milliseconds, GhostStatus status)
        {
            return new object[] { level, milliseconds, status };
        }
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return CreateTestData(1, PercentagesInMilliseconds.Percent50, GhostStatus.Edible);
            yield return CreateTestData(1, PercentagesInMilliseconds.Percent50, GhostStatus.Flash);

            for (var level = 2; level <= 4; level++)
            {
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent55, GhostStatus.Edible);
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent55, GhostStatus.Flash);
            }

            for (var level = 5; level <= 256; level++)
            {
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent60, GhostStatus.Edible);
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent60, GhostStatus.Flash);
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class GhostShouldTravelAtDifferentSpeedsForDifferentLevelsWhenAliveTestData : IEnumerable<object[]>
    {
        private object[] CreateTestData(int level, int milliseconds)
        {
            return new object[] { level, milliseconds };
        }
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return CreateTestData(1, PercentagesInMilliseconds.Percent75);

            for (var level = 2; level <= 4; level++)
            {
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent85);
            }

            for (var level = 5; level <= 256; level++)
            {
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent95);
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class GhostShouldTravelAtDifferentSpeedsForDifferentLevelsWhenRunningHomeTestData : IEnumerable<object[]>
    {
        private object[] CreateTestData(int level, int milliseconds)
        {
            return new object[] { level, milliseconds };
        }
        public IEnumerator<object[]> GetEnumerator()
        {
            for (var level = 1; level <= 256; level++)
            {
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent160);
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class BlinkyShouldTravelAtDifferentSpeedsDuringCruiseElroyTestData : IEnumerable<object[]>
    {
        private IEnumerable<(int coinsLeft, int milliseconds)> CreateCoins(
            int elroyPoint, int lowerMilliseconds, int midMilliseconds, int upperMilliseconds)
        {
            var halfElroyPoint = elroyPoint / 2;
            for (var i = 1; i <= halfElroyPoint; i++)
            {
                yield return (i, lowerMilliseconds);
            }
            for (var i = halfElroyPoint + 1; i < elroyPoint; i++)
            {
                yield return (i, midMilliseconds);
            }
            for (var i = elroyPoint; i < 150; i++)
            {
                yield return (i, upperMilliseconds);
            }
        }
        private object[] CreateTestData(int level, int coinsLeft, int milliseconds)
        {
            return new object[] { level, coinsLeft, milliseconds };
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            // Level 1
            foreach (var (coinsLeft, milliseconds) in CreateCoins(20, PercentagesInMilliseconds.Percent85, PercentagesInMilliseconds.Percent80, PercentagesInMilliseconds.Percent75))
            {
                yield return CreateTestData(1, coinsLeft, milliseconds);
            }

            // Level 2
            foreach (var (coinsLeft, milliseconds) in CreateCoins(30, PercentagesInMilliseconds.Percent95, PercentagesInMilliseconds.Percent90, PercentagesInMilliseconds.Percent85))
            {
                yield return CreateTestData(2, coinsLeft, milliseconds);
            }


            for (var level = 3; level <= 4; level++)
            {
                foreach (var (coinsLeft, milliseconds) in CreateCoins(40, PercentagesInMilliseconds.Percent95, PercentagesInMilliseconds.Percent90, PercentagesInMilliseconds.Percent85))
                {
                    yield return CreateTestData(level, coinsLeft, milliseconds);
                }
            }

            foreach (var (coinsLeft, milliseconds) in CreateCoins(40, PercentagesInMilliseconds.Percent105, PercentagesInMilliseconds.Percent100, PercentagesInMilliseconds.Percent95))
            {
                yield return CreateTestData(5, coinsLeft, milliseconds);
            }

            for (var level = 6; level <= 8; level++)
            {
                foreach (var (coinsLeft, milliseconds) in CreateCoins(50, PercentagesInMilliseconds.Percent105, PercentagesInMilliseconds.Percent100, PercentagesInMilliseconds.Percent95))
                {
                    yield return CreateTestData(level, coinsLeft, milliseconds);
                }
            }

            for (var level = 9; level <= 11; level++)
            {
                foreach (var (coinsLeft, milliseconds) in CreateCoins(60, PercentagesInMilliseconds.Percent105, PercentagesInMilliseconds.Percent100, PercentagesInMilliseconds.Percent95))
                {
                    yield return CreateTestData(level, coinsLeft, milliseconds);
                }
            }

            for (var level = 12; level <= 14; level++)
            {
                foreach (var (coinsLeft, milliseconds) in CreateCoins(80, PercentagesInMilliseconds.Percent105, PercentagesInMilliseconds.Percent100, PercentagesInMilliseconds.Percent95))
                {
                    yield return CreateTestData(level, coinsLeft, milliseconds);
                }
            }

            for (var level = 15; level <= 18; level++)
            {
                foreach (var (coinsLeft, milliseconds) in CreateCoins(100, PercentagesInMilliseconds.Percent105, PercentagesInMilliseconds.Percent100, PercentagesInMilliseconds.Percent95))
                {
                    yield return CreateTestData(level, coinsLeft, milliseconds);
                }
            }

            for (var level = 19; level <= 256; level++)
            {
                foreach (var (coinsLeft, milliseconds) in CreateCoins(120, PercentagesInMilliseconds.Percent105, PercentagesInMilliseconds.Percent100, PercentagesInMilliseconds.Percent95))
                {
                    yield return CreateTestData(level, coinsLeft, milliseconds);
                }
            }

        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class PercentagesInMilliseconds
    {
        // Percent => Milliseconds : m = 100 (should be 107) / (p / 100)
        public const int Percent50 = 200; // 214
        public const int Percent55 = 181; // 194
        public const int Percent60 = 166; // 178
        public const int Percent75 = 133; // 142
        public const int Percent80 = 125; // 133
        public const int Percent85 = 117; // 125
        public const int Percent90 = 111; // 118
        public const int Percent95 = 105; // 112
        public const int Percent100 = 100; // 107
        public const int Percent105 = 95; // 101
        public const int Percent160 = 62; // 66
    }

    public class MoveClockTests
    {
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
                   moveClock => moveClock.ShouldGhostMove(level, coinsRemaining: int.MaxValue, ghostName, ghostStatus));
        }

        [Theory]
        [ClassData(typeof(GhostShouldTravelAtDifferentSpeedsForDifferentLevelsWhenAliveTestData))]
        public void GhostShouldTravelAtDifferentSpeedsForDifferentLevelsWhenAlive(int level, int moveSpeedMilliseconds)
        {
            var ghostName = Guid.NewGuid().ToString();

            TestMoveClock(moveSpeedMilliseconds,
                   moveClock => moveClock.ShouldGhostMove(level, coinsRemaining: int.MaxValue, ghostName, GhostStatus.Alive));
        }

        [Theory]
        [ClassData(typeof(GhostShouldTravelAtDifferentSpeedsForDifferentLevelsWhenRunningHomeTestData))]
        public void GhostShouldTravelAtDifferentSpeedsForDifferentLevelsWhenRunningHome(int level, int moveSpeedMilliseconds)
        {
            var ghostName = Guid.NewGuid().ToString();

            TestMoveClock(moveSpeedMilliseconds,
                   moveClock => moveClock.ShouldGhostMove(level, coinsRemaining: int.MaxValue, ghostName, GhostStatus.RunningHome));
        }

        [Theory]
        // [InlineData(4, 20, PercentagesInMilliseconds.Percent85)]
        // [InlineData(4, 19, PercentagesInMilliseconds.Percent90)]
        // [InlineData(4, 11, PercentagesInMilliseconds.Percent90)]
        // [InlineData(4, 10, PercentagesInMilliseconds.Percent95)]

        // [InlineData(5, 20, PercentagesInMilliseconds.Percent95)]
        // [InlineData(5, 19, PercentagesInMilliseconds.Percent100)]
        // [InlineData(5, 11, PercentagesInMilliseconds.Percent100)]
        // [InlineData(5, 10, PercentagesInMilliseconds.Percent105)]
        //[InlineData(4, PercentagesInMilliseconds.Percent160)]
        //[InlineData(5, PercentagesInMilliseconds.Percent160)]
        [ClassData(typeof(BlinkyShouldTravelAtDifferentSpeedsDuringCruiseElroyTestData))]
        public void BlinkyShouldTravelAtDifferentSpeedsDuringCruiseElroy(int level, int coinsLeft, int moveSpeedMilliseconds)
        {
            var ghostName = GhostNames.Blinky;
            TestMoveClock(moveSpeedMilliseconds,
                   moveClock => moveClock.ShouldGhostMove(level, coinsLeft, ghostName, GhostStatus.Alive));
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