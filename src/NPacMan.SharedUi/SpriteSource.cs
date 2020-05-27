namespace NPacMan.SharedUi
{
    public readonly struct SpriteSource
    {
        public int XPos { get; }
        public int YPos { get; }
        public int Size { get; }

        public SpriteSource(int x, int y, int size)
        {
            XPos = x;
            YPos = y;
            Size = size;
        }

        public override bool Equals(object? obj)
        {
            if (obj is SpriteSource spriteSource)
            {
                return Equals(spriteSource);
            }
            return false;
        }

        public bool Equals(SpriteSource spriteSource)
        {
            return (XPos == spriteSource.XPos) 
                   && (YPos == spriteSource.YPos)
                && (Size == spriteSource.Size);
        }

        public override int GetHashCode()
        {
            return XPos ^ YPos  ^ Size;
        }

        public static bool operator ==(SpriteSource lhs, SpriteSource rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SpriteSource lhs, SpriteSource rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}