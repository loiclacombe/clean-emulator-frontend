using System.Diagnostics;

namespace Launchers
{
    public class ExitedProcess
    {
        private readonly string _errorMessage;
        private readonly int _exitCode;
        private readonly ProcessStartInfo _startInfo;

        public ExitedProcess(Process process)
        {
            _exitCode = process.ExitCode;
            _errorMessage = process.StandardError.ReadToEnd();
            _startInfo = process.StartInfo;
        }

        public ProcessStartInfo StartInfo
        {
            get { return _startInfo; }
        }

        public int ExitCode
        {
            get { return _exitCode; }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }
    }
}