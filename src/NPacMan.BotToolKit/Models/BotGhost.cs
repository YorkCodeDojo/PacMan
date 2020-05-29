﻿namespace NPacMan.BotSDK.Models
{
    public class BotGhost
    {
        public string Name { get; set; } = default!;

        public bool Edible { get; set; }

        public CellLocation Location { get; set; }
    }
}
