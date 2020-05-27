using NPacMan.Game;

namespace NPacMan.UI.Bots
{
    public class BotGhost
    {
        public string Name { get; set; } = default!;

        public bool Edible { get; set; }

        public BotLocation Location { get; set; }
    }
}
