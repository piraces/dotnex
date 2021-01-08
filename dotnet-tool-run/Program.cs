using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace dotnettoolrun
{
    public class Program
    {
        public static int Main(string[] args)
        {
            
            var rootCommand = new RootCommand
            {
                new Option<string>(
                    new[]{ "--version", "-v" },
                    "Version of the tool to use"),
                new Option<string>(
                    new[]{ "--framework", "-f" },
                    "Target framework for the tool"),
                new Argument<string>("TOOL", "The NuGet Package Id of the tool to execute"),
                new Argument<string[]>("TOOL-ARGS", "Arguments to pass to the tool to execute")
            };
            
            rootCommand.Handler = CommandHandler.Create<string, string, string, string[]?>(Execute);
 
            return rootCommand.InvokeAsync(args).Result;
        }

        private static async Task<int> Execute(string tool, string version, string framework, string[]? toolArgs)
        {
            var finalToolArgs = toolArgs != null ? string.Join(' ', toolArgs) : null;
            var toolHandler = new ToolHandler(tool, version, framework, finalToolArgs);
            var existingPublishedTool = await toolHandler.CheckValidTool();
            if (existingPublishedTool)
            {
                return await toolHandler.StartTool();
            }
            Console.WriteLine($"[X] The specified tool '{tool}' does not exist... Please check the correct name at https://www.nuget.org/packages");
            return 1;
        }
    }
}
