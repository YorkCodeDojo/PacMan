namespace NPacMan.UI.Bots
{
    internal interface IBotTransport
    {
        string SendCommand(string payload);
    }
}