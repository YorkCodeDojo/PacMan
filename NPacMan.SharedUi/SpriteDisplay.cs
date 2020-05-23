using System;
using System.Collections.Generic;
using System.Text;

namespace NPacMan.SharedUi
{
    public readonly struct SpriteDisplay
    {
        public decimal XPos { get; }
        public decimal YPos { get; }
        public SpriteSource Sprite { get; }

        public SpriteDisplay(decimal x, decimal y, SpriteSource sprite)
        {
            XPos = x;
            YPos = y;
            Sprite = sprite;
        }
    }
}
