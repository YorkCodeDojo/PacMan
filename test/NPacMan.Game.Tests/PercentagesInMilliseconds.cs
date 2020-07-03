namespace NPacMan.Game.Tests
{
    public static class PercentagesInMilliseconds
    {
        // Percent => Milliseconds : m = 107 / (p / 100)
        // 107 is 1000/60 (FPS) * 8 as we move one cell (8 pixels) at a time
        public const int Percent40 = 267;
        public const int Percent45 = 237;
        public const int Percent50 = 214; // 214
        public const int Percent55 = 194; // 194
        public const int Percent60 = 178; // 178
        public const int Percent75 = 142; // 142
        public const int Percent80 = 133; // 133
        public const int Percent85 = 125; // 125
        public const int Percent90 = 118; // 118
        public const int Percent95 = 112; // 112
        public const int Percent100 = 107; // 107
        public const int Percent105 = 101; // 101
        public const int Percent160 = 66; // 66
    }
}