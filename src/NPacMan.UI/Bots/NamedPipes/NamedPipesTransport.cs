using System.IO.Pipes;

namespace NPacMan.UI.Bots
{
    internal class NamedPipesTransport : IBotTransport
    {
        private StreamString? _pipeStream;
        private readonly object _lock = new object();
        private readonly string _pipeName;

        public NamedPipesTransport(string pipeName)
        {
            _pipeName = pipeName;
            _pipeStream = ConnectToClient();
        }

        private StreamString ConnectToClient()
        {
            var pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, 1);
            pipeServer.WaitForConnection();

            var pipeStream = new StreamString(pipeServer);

            return pipeStream;
        }

        public string SendCommand(string payload)
        {
            try
            {
                if (_pipeStream is null)
                {
                    // We previously lost connection - try and get it back!
                    _pipeStream = ConnectToClient();
                }

                lock (_lock)
                {
                    _pipeStream!.WriteString(payload);

                    return _pipeStream.ReadString();
                }
            }
            catch
            {
                // We have lost connection
                _pipeStream = null;
            }

            return string.Empty;
        }
    }
}
