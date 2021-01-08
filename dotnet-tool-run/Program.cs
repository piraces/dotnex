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
                    "--version",
                    "Version of the tool to use"),
                new Option<string>(
                    "--framework",
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
            return await toolHandler.StartTool();
        }
    }
}
