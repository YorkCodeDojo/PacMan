using System.Collections.Generic;

namespace NPacMan.Game
{
    public interface IGameSettings
    {
        PacMan PacMan { get; }
        IReadOnlyCollection<(int x, int y)> Walls { get; }
        IReadOnlyCollection<CellLocation> Doors { get; }
        IReadOnlyCollection<(int x, int y)> Coins { get; }
        IReadOnlyDictionary<(int x, int y), (int x, int y)> Portals { get; }
        int Width { get; }
        int Height { get; }
        IReadOnlyCollection<Ghost> Ghosts { get; }
    }
}