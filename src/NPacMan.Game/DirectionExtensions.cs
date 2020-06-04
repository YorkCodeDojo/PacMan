using Automatonymous.Contexts;
using System;

namespace NPacMan.Game
{
    public static class DirectionExtensions
    {
        public static Direction Opposite(this Direction direction)
            => direction switch
            {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Right => Direction.Left,
                Direction.Left => Direction.Right,
                _ => throw new NotImplementedException()
            };

        private static class DirectionText
        {
            public const string Down = "Down";
            public const string Up = "Up";
            public const string Left = "Left";
            public const string Right = "Right";
        }

        public static string ToText(this Direction direction)
            => direction switch
            {
                Direction.Up => DirectionText.Down,
                Direction.Down => DirectionText.Up,
                Direction.Right => DirectionText.Left,
                Direction.Left => DirectionText.Right,
                _ => string.Empty
            };
    }
}
