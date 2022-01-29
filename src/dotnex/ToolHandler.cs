using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace dotnex
{
    /// <summary>
    /// Class that handles the download, install and execution processes for dotnet tools.
    /// </summary>
    public class ToolHandler
    {
        private const string DEFAULT_NUGET_FEED = "https://www.nuget.org/packages/";

        private readonly string[] _toolManifestCliProcessArgs = { "new", "tool-manifest", "--force" };

        private readonly CliCommandLineWrapper _dotnetManifestCommand;
        private readonly CliCommandLineWrapper _dotnetInstallCommand;
        private static HttpClient _httpClient = new();
        
        private string _toolName;
        private string? _toolArgs;
        private bool _removeCache;
        private string _tempFolder;

        /// <summary>
        /// Constructor for ToolHandler.
        /// Initializes the tool with its name, version, target framework and arguments/options to execute the tool with.
        /// </summary>
        /// <param name="toolName">Name of the dotnet tool</param>
        /// <param name="version">Version of the dotnet tool to download, install and execute (default latest)</param>
        /// <param name="framework">Target framework for the tool (default same as the current dotnet sdk used to run this process)</param>
        /// <param name="removeCache">If true, cache is purged is exists for this tool. Otherwise it will use cache if exists (default false)</param>
        /// <param name="toolArgs">Options/arguments to invoke the tool with (default none)</param>
        public ToolHandler(string toolName, string? version = null, string? framework = null, bool removeCache = false,
            string? toolArgs = null)
        {
            _toolName = toolName;
            _toolArgs = toolArgs;
            _removeCache = removeCache;
            _tempFolder = CacheManager.GetTempFolder();
            _dotnetManifestCommand = new CliCommandLineWrapper(_toolManifestCliProcessArgs, true);
            var installArguments = GetToolInstallCliProcessArgs(toolName, version, framework);
            _dotnetInstallCommand = new CliCommandLineWrapper(installArguments, true);
        }

        /// <summary>
        /// Starts the execution defined in this class, generating the manifest and removing cache if needed.
        /// </summary>
        /// <returns>Exit code from the execution of the tool</returns>
        public async Task<int> StartTool()
        {
            if (_removeCache)
            {
                CacheManager.RemoveAllCachedFiles();  
            }
            var manifestProcessResult = await _dotnetManifestCommand.StartCliCommand();
            if (manifestProcessResult > 0)
            {
                Console.WriteLine("[X] Could not create manifest for tool. Check if this program is allowed to write in the current directory.");
                return 1;
            }
            await _dotnetInstallCommand.StartCliCommand();

            return await StartToolProcess();
        }

        /// <summary>
        /// Makes a request to the main NuGet package repository and checks if the tool exists or not
        /// </summary>
        /// <returns>True if the tool exists in the NuGet package repository, false otherwise</returns>
        public async Task<bool> CheckValidTool()
        {
            var response = await _httpClient.GetAsync($"{DEFAULT_NUGET_FEED}{_toolName}");
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Gets the install arguments for the dotnet tool with the version and framework specified.
        /// </summary>
        /// <param name="toolName">Name of the dotnet tool to install</param>
        /// <param name="version">Version of the dotnet tool to install (default latest)</param>
        /// <param name="framework">Target framework of the tool to install (default same as the current dotnet sdk used to run this process)</param>
        /// <returns>Array of arguments to install the dotnet tool with the specified options</returns>
        private string[] GetToolInstallCliProcessArgs(string toolName, string? version, string? framework) {
            var args = new[]
            {
                "tool",
                "install",
                toolName,
                "--tool-path",
                _tempFolder
            };

            if (!string.IsNullOrWhiteSpace(version))
            {
                args = args.Concat(new[] {"--version", version}).ToArray();
            }

            if (!string.IsNullOrWhiteSpace(framework))
            {
                args = args.Concat(new[] { "--framework", framework }).ToArray();
            }

            return args;
        }

        /// <summary>
        /// Starts the execution of the external tool downloaded with the provided args/options (synchronous)
        /// </summary>
        /// <returns>Returns the exit code of the execution</returns>
        private async Task<int> StartToolProcess()
        {
            var toolProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(_tempFolder, _toolName), WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = _toolArgs ?? string.Empty
                }
            };
            toolProcess.Start();

#if NET5_0_OR_GREATER
            await toolProcess.WaitForExitAsync();
#else
            await Task.Run(() => toolProcess.WaitForExit()); // TODO
#endif

            return toolProcess.ExitCode;
        }
    }
}
