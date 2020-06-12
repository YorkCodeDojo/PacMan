using System;
using System.Collections.Generic;

namespace NPacMan.Game
{
    public static class Fruits
    {
        private static readonly Dictionary<int, FruitType> _fruitByLevels = new Dictionary<int, FruitType>
        {
            [1] = FruitType.Cherry,
            [2] = FruitType.Strawberry,
            [3] = FruitType.Orange,
            [4] = FruitType.Orange,
            [5] = FruitType.Bell,
            [6] = FruitType.Bell,
            [7] = FruitType.Apple,
            [8] = FruitType.Apple,
            [9] = FruitType.Grapes,
            [10] = FruitType.Grapes,
            [11] = FruitType.Arcadian,
            [12] = FruitType.Arcadian
        };

        public static FruitDetails FruitForLevel(int levelNumber)
        {
            if (!_fruitByLevels.TryGetValue(levelNumber, out var fruitType))
            {
                fruitType = FruitType.Key;
            }

            var scoreIncrement = fruitType switch
            {
                FruitType.Cherry => 100,
                FruitType.Strawberry => 300,
                FruitType.Orange => 500,
                FruitType.Bell => 700,
                FruitType.Apple => 1000,
                FruitType.Grapes => 2000,
                FruitType.Arcadian => 3000,
                FruitType.Key => 5000,
                _ => throw new NotImplementedException()
            };

            return new FruitDetails(fruitType, scoreIncrement);
        }
    }
}