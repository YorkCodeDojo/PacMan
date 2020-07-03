using System.Collections;
using System.Collections.Generic;

namespace NPacMan.Game.Tests
{
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

            for (var level = 5; level <= MoveClockTests.MaxLevel; level++)
            {
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent95);
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}