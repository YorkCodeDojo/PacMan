namespace NPacMan.BotSDK
{
    public class BotGame
    {
        public CellLocation[] Coins { get; set; } = default!;

        public CellLocation[] PowerPills { get; set; } = default!;

        public CellLocation[] Walls { get; set; } = default!;

        public CellLocation[] Doors { get; set; } = default!;

        public BotPortal[] Portals { get; set; } = default!;

        public int Width { get; set; }

        public int Height { get; set; }

        public int Lives { get; set; }

        public int Score { get; set; }

        public BotPacMan PacMan { get; set; } = default!;

        public BotGhost[] Ghosts { get; set; } = default!;
    }
}
