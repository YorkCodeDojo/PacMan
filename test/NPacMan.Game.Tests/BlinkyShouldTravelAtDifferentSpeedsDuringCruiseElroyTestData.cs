using System.Collections;
using System.Collections.Generic;

namespace NPacMan.Game.Tests
{
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
            for (var i = elroyPoint; i < MoveClockTests.MaxCoins; i++)
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

            for (var level = 19; level <= MoveClockTests.MaxLevel; level++)
            {
                foreach (var (coinsLeft, milliseconds) in CreateCoins(120, PercentagesInMilliseconds.Percent105, PercentagesInMilliseconds.Percent100, PercentagesInMilliseconds.Percent95))
                {
                    yield return CreateTestData(level, coinsLeft, milliseconds);
                }
            }

        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}