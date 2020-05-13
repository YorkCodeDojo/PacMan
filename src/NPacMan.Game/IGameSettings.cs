using System.Collections.Generic;

namespace NPacMan.Game
{
    public interface IGameSettings
    {
        PacMan PacMan { get; }
        IReadOnlyCollection<CellLocation> Walls { get; }
        IReadOnlyCollection<CellLocation> Doors { get; }
        IReadOnlyCollection<CellLocation> Coins { get; }
        IReadOnlyDictionary<CellLocation, CellLocation> Portals { get; }
        int Width { get; }
        int Height { get; }
        IReadOnlyCollection<Ghost> Ghosts { get; }
        GameStatus InitialGameStatus { get; }
        int InitialLives { get; }

        int InitialScatterTimeInSeconds { get; }

        int ChaseTimeInSeconds { get; }

    }
}