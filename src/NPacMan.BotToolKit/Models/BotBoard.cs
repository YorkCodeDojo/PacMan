namespace NPacMan.BotSDK.Models
{
    public class BotBoard
    {
        public CellLocation[] Walls { get; set; } = default!;
        
        public BotPortal[] Portals { get; set; } = default!;

        public int Width { get; set; }

        public int Height { get; set; }

    }
}
