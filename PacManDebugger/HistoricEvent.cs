namespace PacManDebugger
{
    public readonly struct HistoricEvent
    {
        public Location OriginalLocation { get; }
        public Location FinalLocation { get; }
        public bool WasMoveEvent { get; }

        public HistoricEvent(Location originalLocation, Location finalLocation,  bool wasMoveEvent)
        {
            OriginalLocation = originalLocation;
            FinalLocation = finalLocation;
            WasMoveEvent = wasMoveEvent;
        }
    }


}
