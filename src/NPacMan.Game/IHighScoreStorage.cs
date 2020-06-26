using System.Threading.Tasks;

namespace NPacMan.Game
{
    public interface IHighScoreStorage
    {
        Task<int> GetHighScore();
        Task SetHighScore(int highScore);
    }
}