using NPacMan.Game;
using System.Collections.Generic;

namespace NPacMan.UI
{
    public class LinkedCell
    {
        public CellLocation Location { get; }
        public LinkedCell? Left { get; private set; }
        public LinkedCell? Right { get; private set; }
        public LinkedCell? Up { get; private set; }
        public LinkedCell? Down { get; private set; }
        public (Direction Direction, LinkedCell NewLocation)[] AvailableMoves { get; private set; } = default!;

        public LinkedCell(CellLocation location)
        {
            Location = location;
        }

        public void DefineNeighbours(LinkedCell? left, LinkedCell? right, LinkedCell? up, LinkedCell? down)
        {
            Left = left;
            Right = right;
            Up = up;
            Down = down;

            var tempAvailableMoves = new List<(Direction, LinkedCell)>(4);

            if (left is object)
            {
                tempAvailableMoves.Add((Direction.Left, left));
            }

            if (right is object)
            {
                tempAvailableMoves.Add((Direction.Right, right));
            }

            if (up is object)
            {
                tempAvailableMoves.Add((Direction.Up, up));
            }

            if (down is object)
            {
                tempAvailableMoves.Add((Direction.Down, down));
            }

            AvailableMoves = tempAvailableMoves.ToArray();
        }
    }
}