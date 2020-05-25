using System.Collections.Generic;

namespace NPacMan.Game
{
    public interface IDirectionPicker
    {
        Direction Pick(IEnumerable<Direction> directions);
    }
}
