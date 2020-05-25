using NPacMan.Game;
using System.Collections.Generic;

namespace NPacMan.UI.Bots
{
    public class BotGame
    {
        public IEnumerable<CellLocation> Coins { get; set; } = default!;

        public IEnumerable<CellLocation> PowerPills { get; set; } = default!;

        public IEnumerable<CellLocation> Doors { get; set; } = default!;

        public BotBoard Board { get; set; } = default!;

        public int Lives { get; set; }

        public int Score { get; set; }

        public BotPacMan PacMan { get; set; } = default!;

        public IEnumerable<BotGhost> Ghosts { get; set; } = default!;
    }
}
