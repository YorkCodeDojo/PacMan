namespace NPacMan.Game
{
    public interface ISoundSet
    {
        void Chomp();
        void EatFruit();
        void Beginning();
        void Death();
        void Intermission();
        void ExtraPac();
        void EatGhost();
    }
}