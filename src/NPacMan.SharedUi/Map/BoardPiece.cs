namespace NPacMan.SharedUi.Map
{
    internal enum BoardPiece
    {
        Blank, Pill, PowerAnim1, PowerAnim2,
        DoubleTopRight, DoubleTopLeft, DoubleRight, DoubleLeft, DoubleBottomRight, DoubleBottomLeft,
        JoinRightHandTop, JoinLeftHandTop, JoinRightHandBottom, JoinLeftHandBottom, DoubleTop, DoubleBottom, Top,
        Bottom, TopRight, TopLeft, Right, Left, BottomRight, BottomLeft, 
        GhostTopRight, GhostTopLeft, GhostBottomRight, GhostBottomLeft, GhostEndLeft, GhostEndRight,
        JoinTopRight, JoinTopLeft, GhostDoor,
        InnerTopRight, InnerTopLeft, InnerBottomRight, InnerBottomLeft,
        InsideWalls, Undefined,
        JoinBottomRight, JoinBottomLeft
    }
}