namespace NPacMan.Game
{
    public interface IHighScoreStorage
    {
        int GetHighScore();
        void SetHighScore(int highScore);
    }
}