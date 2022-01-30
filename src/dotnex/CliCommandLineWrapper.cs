using System.Diagnostics;
using System.Threading.Tasks;

namespace dotnex
{
    /// <summary>
    /// Wrapper around the dotnet cli to execute external tools.
    /// </summary>
    public class CliCommandLineWrapper
    {
        private const string DOTNET_CLI_COMMAND = "dotnet";

        private readonly Process _toolCliProcess;

        /// <summary>
        /// Constructor for the CLI wrapper.
        /// It takes arguments, if the output should be redirected, the working directory and if the external tool window should be hidden.
        /// </summary>
        /// <param name="args">Arguments/options to execute the tool with</param>
        /// <param name="redirectOutput">If the STDERR/STDOUT should be redirected (default false)</param>
        /// <param name="workingDirectory">Working directory to run the tool from (default the current one)</param>
        /// <param name="processWindowStyle">ProcessWindowStyle for the CLI execution (default hidden)</param>
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
        /// Starts the CLI command and returns the exit code.
        /// </summary>
        /// <returns>The exit code from the CLI command execution</returns>
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
