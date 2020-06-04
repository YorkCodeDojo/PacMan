namespace PacManDebugger
{
    public readonly struct HistoricPacManStateChangeEvent
    {
        public bool EventSet { get; }
        public int Lives { get; }
        public int Score { get; }
        public string Direction { get; }

        public HistoricPacManStateChangeEvent(int lives, int score, string direction)
        {
            EventSet = true;
            Lives = lives;
            Score = score;
            Direction = direction;
        }
    }
}
