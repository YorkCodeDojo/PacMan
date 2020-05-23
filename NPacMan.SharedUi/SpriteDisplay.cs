using System;
using System.Collections.Generic;
using System.Text;

namespace NPacMan.SharedUi
{
    public readonly struct SpriteDisplay
    {
        public int XPos { get; }
        public int YPos { get; }
        public SpriteSource Sprite { get; }

        public SpriteDisplay(int x, int y, SpriteSource sprite)
        {
            XPos = x;
            YPos = y;
            Sprite = sprite;
        }
    }
}
