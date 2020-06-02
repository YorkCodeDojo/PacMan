using System;

namespace PacManDebugger
{
    class Board
    {
        public int Width { get; private set; } = 50;
        public int Height { get; private set; } = 50;
        public CellLocation[] Walls { get; private set; } = Array.Empty<CellLocation>();

        public void UpdateDefinition(int width, int height, CellLocation[] walls)
        {
            Width = width;
            Height = height;
            Walls = walls;
        }

        internal void Clear()
        {
            Walls = Array.Empty<CellLocation>();
            Width = 50;
            Height = 50;
        }
    }
}
