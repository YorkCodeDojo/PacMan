namespace NPacMan.BotSDK.Models
{
    public class BotGame
    {
        public CellLocation[] Coins { get; set; } = default!;

        public CellLocation[] PowerPills { get; set; } = default!;

        public CellLocation[] Doors { get; set; } = default!;

        public int Lives { get; set; }

        public int Score { get; set; }

        public BotPacMan PacMan { get; set; } = default!;

        public BotGhost[] Ghosts { get; set; } = default!;

        public BotBoard Board { get; set; } = default!;
    }
}
