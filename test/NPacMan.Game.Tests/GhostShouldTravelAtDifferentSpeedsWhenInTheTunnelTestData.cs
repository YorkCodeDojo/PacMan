using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NPacMan.Game.Tests
{
    public class GhostShouldTravelAtDifferentSpeedsWhenInTheTunnelTestData : IEnumerable<object[]>
    {
        private IEnumerable<object[]> GetDataForStatus(GhostStatus status)
        {
            yield return CreateTestData(1, PercentagesInMilliseconds.Percent40, status);

            for (var level = 2; level <= 4; level++)
            {
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent45, status);
            }

            for (var level = 5; level <= MoveClockTests.MaxLevel; level++)
            {
                yield return CreateTestData(level, PercentagesInMilliseconds.Percent50, status);
            }
        }
        private object[] CreateTestData(int level, int milliseconds, GhostStatus status)
        {
            return new object[] { level, milliseconds, status };
        }
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var status in Enum.GetValues(typeof(GhostStatus)).Cast<GhostStatus>())
            {
                foreach (var testData in GetDataForStatus(status))
                {
                    yield return testData;
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}