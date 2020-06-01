namespace NPacMan.UI.Bots.Models
{
    public class BotGhost
    {
        public string Name { get; set; } = default!;

        public bool Edible { get; set; }

        public BotLocation Location { get; set; }
    }
}
