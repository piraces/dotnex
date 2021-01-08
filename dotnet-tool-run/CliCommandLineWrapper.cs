using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace dotnettoolrun
{
    public class CliCommandLineWrapper
    {
        private const string DOTNET_CLI_COMMAND = "dotnet";

        private readonly Process _toolCliProcess;

        public CliCommandLineWrapper(string[] args, bool redirectOutput = false, string? workingDirectory = null, ProcessWindowStyle processWindowStyle = ProcessWindowStyle.Hidden)
        {
            _toolCliProcess = new Process();
            _toolCliProcess.StartInfo = new ProcessStartInfo
            {
                FileName = DOTNET_CLI_COMMAND,
                RedirectStandardError = redirectOutput,
                RedirectStandardOutput = redirectOutput,
                WindowStyle = processWindowStyle
            };
            _toolCliProcess.StartInfo.Arguments = string.Join(' ', args);

            if(!string.IsNullOrWhiteSpace(workingDirectory))
            {
                _toolCliProcess.StartInfo.WorkingDirectory = workingDirectory;
            }
        }

        public async Task<int> StartCliCommand()
        {
            _toolCliProcess.Start();
            await _toolCliProcess.WaitForExitAsync();
            return _toolCliProcess.ExitCode;
        }
    }
}
