using System;

namespace NPacMan.Game
{
    public static class DirectionExtentions
    {
        public static Direction Opposite(this Direction direction)
            => direction switch {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Right => Direction.Left,
                Direction.Left => Direction.Right,
                _ => throw new NotImplementedException()
            };
    }
}
