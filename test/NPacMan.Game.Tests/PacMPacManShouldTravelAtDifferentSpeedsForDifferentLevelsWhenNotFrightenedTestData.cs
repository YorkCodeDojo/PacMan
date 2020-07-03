using System.Collections;
using System.Collections.Generic;

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

            for (var level = 21; level <= MoveClockTests.MaxLevel; level++)
            {
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent90);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}