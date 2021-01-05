using System;
using System.Linq;

namespace dotnettoolrun
{
    public class Program
    {
        public static int Main(string[] args)
        {
            string tool = string.Empty;
            string[] cliArgs = Array.Empty<string>();
            string[] readedArgs = ReadInputRedirectedArgs() ?? args;
            
            if (readedArgs.Length == 1)
            {
                tool = readedArgs[0];
            }
            else if(readedArgs.Length > 1)
            {
                tool = readedArgs[0];
                cliArgs = readedArgs.Skip(0).ToArray();
            }

            if (string.IsNullOrWhiteSpace(tool))
            {
                Console.WriteLine("[X] A valid tool name must be provided");
                return 1;
            }

            var cliWrapper = new CliCommandLineWrapper(tool, cliArgs);
            cliWrapper.InstallTempTool();
            return cliWrapper.StartTempTool();
        }

        private static string[]? ReadInputRedirectedArgs()
        {
            if (Console.IsInputRedirected)
            {
                var input = Console.In.ReadToEnd();
                return input.Split(' ');

            }
            return null;
        }
    }
}
