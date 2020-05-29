namespace NPacMan.Game.Tests.Helpers
{
    internal static class CellLocationExtensions
    {
        /// <summary>
        ///     Returns a location far away from the supplied one.  This is used to ensure that 
        ///     PacMan and a ghost don't meet accidently.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static CellLocation FarAway(this CellLocation location) => new CellLocation(location.X + 10, location.Y + 10);

    }
}
