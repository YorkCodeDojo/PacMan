namespace NPacMan.Game
{
    public class FruitDetails
    {
        public FruitType FruitType { get; }

        public int Score { get; }

        public FruitDetails(FruitType fruitType, int score)
        {
            FruitType = fruitType;
            Score = score;
        }
    }
}