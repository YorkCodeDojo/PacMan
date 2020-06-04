namespace PacManDebugger
{
    public readonly struct HistoricPacManMovementEvent
    {
        public CellLocation OriginalLocation { get; }
        public CellLocation FinalLocation { get; }
        public bool EventSet { get; }

        public HistoricPacManMovementEvent(CellLocation originalLocation, CellLocation finalLocation)
        {
            OriginalLocation = originalLocation;
            FinalLocation = finalLocation;
            EventSet = true;
        }
    }
}
