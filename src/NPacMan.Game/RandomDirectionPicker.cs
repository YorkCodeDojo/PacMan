using System.Collections.Generic;
using System.Linq;

namespace NPacMan.Game
{
    public class RandomDirectionPicker : IDirectionPicker
    {
        private int _count = 0;
        public Direction Pick(IReadOnlyCollection<Direction> directions)
        {
            var random = _count % directions.Count();
            _count++;

            return directions.ElementAt(random);
        }
    }
}