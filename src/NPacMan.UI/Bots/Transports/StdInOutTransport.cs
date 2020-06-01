using System.Diagnostics;
using System.IO;
using System.Threading;

namespace NPacMan.UI.Bots.Transports
{
    class StdInOutTransport : IBotTransport
    {
        private readonly Process _process;
        private readonly StreamWriter _streamWriter;
        private readonly AutoResetEvent _outputWaitHandle = new AutoResetEvent(false);
        private string _response = string.Empty;
        public StdInOutTransport(string pathToExe)
        {
            _process = new Process();
            _process.StartInfo.FileName = pathToExe;
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.RedirectStandardInput = true;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.CreateNoWindow = true;

            _process.OutputDataReceived += _process_OutputDataReceived;

            _process.Start();

            _streamWriter = _process.StandardInput;
            _process.BeginOutputReadLine();
        }

        private void _process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _response = e.Data;
            _outputWaitHandle.Set();
        }

        public string SendCommand(string payload)
        {
            _response = string.Empty;
            _streamWriter.WriteLine(payload);
            _outputWaitHandle.WaitOne();

            return _response;
        }
    }
}
