using NPacMan.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NPacMan.UI
{
    internal class SimpleBot : IBot
    {
        private readonly Game.Game game;

        public SimpleBot(Game.Game game)
        {
            this.game = game;
        }

        public Direction SuggestNextDirection()
        {
            var currentLocation = game.PacMan.Location;
            var availableMoves = GetAvailableMovesForLocation(currentLocation, game.Walls);

            availableMoves.Remove(game.PacMan.Direction.Opposite());

            if (availableMoves.Count() == 1)
                return availableMoves.First();

            var rnd = new Random();
            return availableMoves[rnd.Next(availableMoves.Count)];
        }

        private List<Direction> GetAvailableMovesForLocation(CellLocation location, IReadOnlyCollection<CellLocation> walls)
        {
            var result = new List<Direction>(4);

            if (!walls.Contains(location.Above))
                result.Add(Direction.Up);

            if (!walls.Contains(location.Below))
                result.Add(Direction.Down);

            if (!walls.Contains(location.Left))
                result.Add(Direction.Left);

            if (!walls.Contains(location.Right))
                result.Add(Direction.Right);

            return result;
        }
    }
}