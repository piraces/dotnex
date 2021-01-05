using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;

namespace dotnettoolrun
{
    public class CliCommandLineWrapper
    {
        private const string DOTNET_CLI_COMMAND = "dotnet";
        private const string TEMP_FOLDER_NAME = "dotnetrun";
        private const string TOOL_MANIFEST_CLI_PROCESS_ARGS = "new tool-manifest --force";

        private readonly Process _toolInstallCliProcess;
        private readonly Process _toolManifestCliProcess;
        private readonly Process _toolStartCliProcess;
        

        private string TempPath = Assembly.GetExecutingAssembly().Location;

        public CliCommandLineWrapper(string tool, string[] args, ProcessWindowStyle processWindowStyle = ProcessWindowStyle.Hidden)
        {
            InitializeTempFolder();
            _toolInstallCliProcess = new Process();
            _toolInstallCliProcess.StartInfo = new ProcessStartInfo
            {
                FileName = DOTNET_CLI_COMMAND,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WindowStyle = processWindowStyle
            };
            _toolManifestCliProcess = new Process();
            _toolManifestCliProcess.StartInfo = new ProcessStartInfo
            {
                FileName = DOTNET_CLI_COMMAND,
                Arguments = TOOL_MANIFEST_CLI_PROCESS_ARGS,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WindowStyle = processWindowStyle,
                WorkingDirectory = TempPath
            };
            _toolStartCliProcess = new Process();
            _toolStartCliProcess.StartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(TempPath, tool),
                WindowStyle = processWindowStyle
            };
            SetArgumentsForInstallProcess(tool, args);
        }

        private void SetArgumentsForInstallProcess(string tool, string[] args)
        {
            _toolInstallCliProcess.StartInfo.ArgumentList.Add("tool");
            _toolInstallCliProcess.StartInfo.ArgumentList.Add("install");
            _toolInstallCliProcess.StartInfo.ArgumentList.Add(tool);
            _toolInstallCliProcess.StartInfo.ArgumentList.Add("--tool-path");
            _toolInstallCliProcess.StartInfo.ArgumentList.Add(TempPath);
            _toolInstallCliProcess.StartInfo.ArgumentList.Concat(args);
        }

        private void InitializeTempFolder()
        {
            try
            {
                TempPath = Path.GetTempPath();
            } catch (SecurityException)
            {
                Console.WriteLine("[X] Could not obtain access to temp path...");
                Console.WriteLine("[i] Using current directory");
            } finally
            {
                TempPath = Path.Combine(TempPath, TEMP_FOLDER_NAME);
            }
        }

        public void InstallTempTool()
        {
            _toolManifestCliProcess.Start();
            _toolManifestCliProcess.WaitForExit();
            _toolInstallCliProcess.Start();
            _toolInstallCliProcess.WaitForExit();
        }

        public int StartTempTool()
        {
            _toolStartCliProcess.Start();
            _toolStartCliProcess.WaitForExit();
            return _toolStartCliProcess.ExitCode;
        }
    }
}
