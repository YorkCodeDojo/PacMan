namespace NPacMan.Game
{
    public class InMemoryHighScoreStorage : IHighScoreStorage
    {
        private int _highScore = 0;
        public int GetHighScore()
            => _highScore;

        public void SetHighScore(int highScore)
            => _highScore = highScore;
    }
}