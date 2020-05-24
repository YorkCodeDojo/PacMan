using NPacMan.Game;
using System.Collections.Generic;

namespace NPacMan.UI.Bots
{
    public class BotGame
    {
        public IEnumerable<CellLocation> Coins { get; set; } = default!;

        public IEnumerable<CellLocation> PowerPills { get; set; } = default!;

        public IEnumerable<CellLocation> Walls { get; set; } = default!;

        public IEnumerable<CellLocation> Doors { get; set; } = default!;

        public IEnumerable<BotPortal> Portals { get; set; } = default!;

        public int Width { get; set; }

        public int Height { get; set; }

        public int Lives { get; set; }

        public int Score { get; set; }

        public BotPacMan PacMan { get; set; } = default!;

        public IEnumerable<BotGhost> Ghosts { get; set; } = default!;
    }

    public class BotPacMan
    {
        public Direction CurrentDirection{ get; set; }

        public CellLocation Location { get; set; }
    }

    public class BotGhost
    {
        public string Name { get; set; } = default!;

        public bool Edible { get; set; }

        public CellLocation Location { get; set; }
    }

    public class BotPortal
    {
        public CellLocation Entry { get; set; }

        public CellLocation Exit { get; set; }
    }
}
