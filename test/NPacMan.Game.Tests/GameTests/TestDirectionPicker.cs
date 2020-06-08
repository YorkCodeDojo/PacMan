using System;
using System.Collections.Generic;
using System.Linq;

namespace NPacMan.Game.Tests.GameTests
{
    public class TestDirectionPicker : IDirectionPicker
    {
        public Direction DefaultDirection { get; set; } = Direction.Right;
        public Direction Pick(IReadOnlyCollection<Direction> directions)
        {
            var rKey = _returnValues.Keys.Where(x => x.Count() == directions.Count() && x.All(y => directions.Contains(y)))
                                .FirstOrDefault();

            if (rKey is null)
            {
                return DefaultDirection;
            }

            return _returnValues[rKey];
        }

        private Dictionary<IEnumerable<Direction>, Direction> _returnValues
            = new Dictionary<IEnumerable<Direction>, Direction>();
        public void Returns(IEnumerable<Direction> input, Direction output)
        {
            _returnValues.Add(input, output);
        }
    }
}
