namespace NPacMan.UI.Bots.Transports
{
    internal interface IBotTransport
    {
        string SendCommand(string payload);
    }
}