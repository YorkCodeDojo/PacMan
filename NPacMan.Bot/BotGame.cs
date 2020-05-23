using System.Collections.Generic;

namespace NPacMan.Bot
{
    public class BotGame
    {
        public CellLocation[] Coins { get; set; } = default!;

        public CellLocation[] PowerPills { get; set; } = default!;

        public CellLocation[] Walls { get; set; } = default!;

        public CellLocation[] Doors { get; set; } = default!;

        public CellLocation[] PortalEntrances { get; set; } = default!;

        public CellLocation[] PortalExits { get; set; } = default!;

        public int Width { get; set; }

        public int Height { get; set; }

        public int Lives { get; set; }

        public int Score { get; set; }

        public CellLocation PacMan { get; set; }

        public BotGhost[] Ghosts { get; set; } = default!;
    }

    public class BotGhost
    {
        public string Name { get; set; } = default!;

        public bool Edible { get; set; }

        public CellLocation Location { get; set; }
    }
}
