using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;

namespace dotnettoolrun
{
    public class ToolHandler
    {
        private const string TEMP_FOLDER_NAME = "dotnettoolrun";

        private readonly string[] _toolManifestCliProcessArgs =
            new string[] { "new", "tool-manifest", "--force" };

        private CliCommandLineWrapper dotnetManifestCommand;
        private CliCommandLineWrapper dotnetInstallCommand;

        private string TempPath = Assembly.GetExecutingAssembly().Location;

        public ToolHandler(string toolName, string version, string framework)
        {
            InitializeTempFolder();

            dotnetManifestCommand = new CliCommandLineWrapper(_toolManifestCliProcessArgs);
            var installArguments = GetToolInstallCliProcessArgs(toolName, version, framework);
            dotnetInstallCommand = new CliCommandLineWrapper(installArguments);
        }

        private void InitializeTempFolder()
        {
            try
            {
                TempPath = Path.GetTempPath();
            }
            catch (SecurityException)
            {
                Console.WriteLine("[X] Could not obtain access to temp path...");
                Console.WriteLine("[i] Using current directory");
            }
            finally
            {
                TempPath = Path.Combine(TempPath, TEMP_FOLDER_NAME);
            }
            
        }

        private string[] GetToolInstallCliProcessArgs(string toolName, string version, string framework) {
            var args = new string[]
            {
                "tool",
                "install",
                toolName,
                "--tool-path",
                TempPath,
            };

            if (!string.IsNullOrWhiteSpace(version))
            {
                args.Concat(new string[] { "--version", version });
            }

            if (!string.IsNullOrWhiteSpace(framework))
            {
                args.Concat(new string[] { "--framework", framework });
            }

            return args;
        }
    }
}
