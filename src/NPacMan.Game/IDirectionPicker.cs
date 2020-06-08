using System.Collections.Generic;

namespace NPacMan.Game
{
    public interface IDirectionPicker
    {
        Direction Pick(IReadOnlyCollection<Direction> directions);
    }
}
