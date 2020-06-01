using System.Collections.Generic;

namespace NPacMan.UI.Bots.Models
{
    public class BotGame
    {
        public IEnumerable<BotLocation> Coins { get; set; } = default!;

        public IEnumerable<BotLocation> PowerPills { get; set; } = default!;

        public IEnumerable<BotLocation> Doors { get; set; } = default!;

        public BotBoard Board { get; set; } = default!;

        public int Lives { get; set; }

        public int Score { get; set; }

        public BotPacMan PacMan { get; set; } = default!;

        public IEnumerable<BotGhost> Ghosts { get; set; } = default!;
    }
}
