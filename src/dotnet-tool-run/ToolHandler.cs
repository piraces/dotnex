using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace dotnettoolrun
{
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

        public async Task<bool> CheckValidTool()
        {
            var response = await _httpClient.GetAsync($"{NUGET_URL}{_toolName}");
            return response.IsSuccessStatusCode;
        }

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
            await toolProcess.WaitForExitAsync();
            return toolProcess.ExitCode;
        }
    }
}
