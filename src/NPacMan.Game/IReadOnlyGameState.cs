using System.Collections.Generic;

namespace NPacMan.Game
{
    internal interface IReadOnlyGameState
    {
        public string Status { get; }

        public int Lives { get; }

        public bool GhostsVisible { get; }

        public int Score { get; }

        public int Level { get; }

        public int TickCounter { get; }

        public IReadOnlyCollection<CellLocation> RemainingCoins { get; }

        public IReadOnlyCollection<CellLocation> RemainingPowerPills { get; }

        public IReadOnlyDictionary<string, Ghost> Ghosts { get; }

        public PacMan PacMan { get; }

        public bool FruitVisible { get; }

        public FruitType FruitTypeToShow { get; }
    }
}