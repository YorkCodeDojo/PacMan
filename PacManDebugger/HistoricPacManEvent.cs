using System.Windows.Forms;

namespace PacManDebugger
{
    public readonly struct HistoricPacManEvent
    {
        public CellLocation OriginalLocation { get; }
        public CellLocation FinalLocation { get; }
        public bool WasMoveEvent { get; }
        public int Lives { get; }
        public int Score { get; }
        public string Direction { get; }

        public HistoricPacManEvent(CellLocation originalLocation, CellLocation finalLocation, bool wasMoveEvent, int lives, int score, string direction)
        {
            OriginalLocation = originalLocation;
            FinalLocation = finalLocation;
            WasMoveEvent = wasMoveEvent;
            Lives = lives;
            Score = score;
            Direction = direction;
        }

        internal HistoricPacManEvent WithFinalLocation(CellLocation finalLocation)
        {
            return new HistoricPacManEvent(OriginalLocation, finalLocation, WasMoveEvent, Lives, Score, Direction);
        }
    }
}
