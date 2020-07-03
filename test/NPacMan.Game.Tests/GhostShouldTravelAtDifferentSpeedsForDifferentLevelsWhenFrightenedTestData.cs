using System.Collections;
using System.Collections.Generic;

namespace NPacMan.Game.Tests
{
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

            for (var level = 5; level <= MoveClockTests.MaxLevel; level++)
            {
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent60, GhostStatus.Edible);
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent60, GhostStatus.Flash);
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}