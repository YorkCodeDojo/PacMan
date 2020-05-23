using System;

namespace NPacMan.Bot
{
    public static class DirectionExtensions
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
