using System.Threading.Tasks;

namespace NPacMan.Game
{
    public class InMemoryHighScoreStorage : IHighScoreStorage
    {
        private int _highScore = 0;
        public Task<int> GetHighScore()
            => Task.FromResult(_highScore);

        public Task SetHighScore(int highScore)
        {
            _highScore = highScore;
            return Task.CompletedTask;
        }
    }
}