using System;

namespace PacManDebugger
{
    public readonly struct HistoricEvent
    {
        public CellLocation OriginalLocation { get; }
        public CellLocation FinalLocation { get; }
        public bool EventSet { get; }

        public HistoricEvent(CellLocation originalLocation, CellLocation finalLocation,  bool eventSet)
        {
            OriginalLocation = originalLocation;
            FinalLocation = finalLocation;
            EventSet = eventSet;
        }

        internal HistoricEvent WithFinalLocation(CellLocation finalLocation)
        {
            return new HistoricEvent(OriginalLocation, finalLocation, EventSet);
        }
    }
}
