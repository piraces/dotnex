using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace dotnex
{
    /// <summary>
    /// 
    /// </summary>
    public class ToolHandler
    {
        private const string NUGET_URL = "https://www.nuget.org/packages/";

        private readonly string[] _toolManifestCliProcessArgs = { "new", "tool-manifest", "--force" };

        private readonly CliCommandLineWrapper _dotnetManifestCommand;
        private readonly CliCommandLineWrapper _dotnetInstallCommand;
        private static HttpClient _httpClient = new();
        
        private string _toolName;
        private string? _toolArgs;
        private bool _removeCache;
        private string _tempFolder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolName"></param>
        /// <param name="version"></param>
        /// <param name="framework"></param>
        /// <param name="removeCache"></param>
        /// <param name="toolArgs"></param>
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
        /// 
        /// </summary>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckValidTool()
        {
            var response = await _httpClient.GetAsync($"{NUGET_URL}{_toolName}");
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolName"></param>
        /// <param name="version"></param>
        /// <param name="framework"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <returns></returns>
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
