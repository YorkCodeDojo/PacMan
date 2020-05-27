using NPacMan.Game;
using System.Collections.Generic;

namespace NPacMan.UI.Bots
{
    public class BotBoard
    {
        public IEnumerable<BotLocation> Walls { get; set; } = default!;

        public IEnumerable<BotPortal> Portals { get; set; } = default!;

        public int Width { get; set; }

        public int Height { get; set; }

    }
}
