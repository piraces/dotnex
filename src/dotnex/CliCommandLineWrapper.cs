using System.Diagnostics;
using System.Threading.Tasks;

namespace dotnex
{
    /// <summary>
    /// 
    /// </summary>
    public class CliCommandLineWrapper
    {
        private const string DOTNET_CLI_COMMAND = "dotnet";

        private readonly Process _toolCliProcess;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="redirectOutput"></param>
        /// <param name="workingDirectory"></param>
        /// <param name="processWindowStyle"></param>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<int> StartCliCommand()
        {
            _toolCliProcess.Start();
#if NET5_0_OR_GREATER
            await _toolCliProcess.WaitForExitAsync();
#else
            await Task.Run(() => _toolCliProcess.WaitForExit()); // TODO
#endif
            return _toolCliProcess.ExitCode;
        }
    }
}
