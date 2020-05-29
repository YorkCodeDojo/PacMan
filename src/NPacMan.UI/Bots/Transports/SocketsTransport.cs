using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NPacMan.UI.Bots.SocketTransport
{
    internal class SocketsTransport : IBotTransport, IDisposable
    {
        private readonly string _hostName;
        private readonly int _portNumber;
        private Socket? _socket;
        private readonly object _lock = new object();

        public SocketsTransport(string hostName, int portNumber)
        {
            _hostName = hostName;
            _portNumber = portNumber;

            _socket = ConnectToSocket();
        }

        public void Dispose()
        {
            if (_socket is object)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                ((IDisposable)_socket).Dispose();
            }
        }

        public string SendCommand(string payload)
        {
            lock (_lock)
            {
                if (_socket is null)
                    _socket = ConnectToSocket();

                var msg = Encoding.ASCII.GetBytes(payload + "<EOF>");
                _socket.Send(msg);

                var bytes = new byte[1024];
                var bytesRec = _socket.Receive(bytes);
                var response = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                return response;
            }
        }

        private Socket ConnectToSocket()
        {
            var ipHostInfo = Dns.GetHostEntry(_hostName);
            var ipAddress = ipHostInfo.AddressList[0];
            var remoteEP = new IPEndPoint(ipAddress, _portNumber);

            var sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(remoteEP);

            return sender;
        }
    }
}
