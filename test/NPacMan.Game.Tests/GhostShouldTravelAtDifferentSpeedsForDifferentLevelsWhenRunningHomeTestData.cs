using System.Collections;
using System.Collections.Generic;

namespace NPacMan.Game.Tests
{
    public class GhostShouldTravelAtDifferentSpeedsForDifferentLevelsWhenRunningHomeTestData : IEnumerable<object[]>
    {
        private object[] CreateTestData(int level, int milliseconds)
        {
            return new object[] { level, milliseconds };
        }
        public IEnumerator<object[]> GetEnumerator()
        {
            for (var level = 1; level <= MoveClockTests.MaxLevel; level++)
            {
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent160);
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}