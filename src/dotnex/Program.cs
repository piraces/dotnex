using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace dotnex
{
    /// <summary>
    /// Main class of dotnex and entry point.
    /// Contains the options specification and description, the main handler and the root command definition (using System.CommandLine).
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point.
        /// Composes the options and arguments, initializes the root command, sets the handler and invoke the root command.
        /// </summary>
        /// <param name="args">Command line arguments passed via STDIN</param>
        /// <returns></returns>
        public static int Main(string[] args)
        {
            var versionOption = new Option<string>(
                    new[] { "--use-version", "-v" },
                    "Version of the tool to use");

            var frameworkOption = new Option<string>(
                    new[] { "--framework", "-f" },
                    "Target framework for the tool");

            var removeCacheOption = new Option<bool>(
                    new[] { "--remove-cache", "-r" },
                    "Flag to remove the local cache before running the tool (can be run without tool)");

            var toolArgument = new Argument<string?>("TOOL", () => null, "The NuGet Package Id of the tool to execute");

            var toolArgsArgument = new Argument<string[]>("TOOL-ARGS", "Arguments to pass to the tool to execute");

            var rootCommand = new RootCommand("Execute other dotnet tools without installing them globally or in a project")
            {
                versionOption,
                frameworkOption,
                removeCacheOption,
                toolArgument,
                toolArgsArgument
            };

            rootCommand.SetHandler(async (InvocationContext invocationContext, string? tool, string version, string framework, bool removeCache, string[]? toolArgs) => {
                await Execute(invocationContext, tool, version, framework, removeCache, toolArgs);
            }, toolArgument, versionOption, frameworkOption, removeCacheOption, toolArgsArgument);

            return rootCommand.Invoke(args);
        }

        /// <summary>
        /// Main method that handles all the inputs and executes the external tool with the provided tool arguments and options.
        /// The external tool downloaded and executed follows the version and target framework specified.
        /// </summary>
        /// <param name="invocationContext">Invocation context from System.CommandLine</param>
        /// <param name="tool">The external dotnet tool to execute</param>
        /// <param name="version">The version of the dotnet tool to download and execute</param>
        /// <param name="framework">The target framework for the dotnet tool</param>
        /// <param name="removeCache">If true, it clears the main cache of dotnex, where dotnet tools are stored for subsequent executions</param>
        /// <param name="toolArgs">Options and arguments to pass to the external dotnet tool</param>
        private static async Task Execute(InvocationContext invocationContext, string? tool, string version, string framework, bool removeCache,
            string[]? toolArgs)
        {
            var finalToolArgs = toolArgs != null ? string.Join(' ', toolArgs) : null;
            if (!string.IsNullOrWhiteSpace(tool))
            {
                var toolHandler = new ToolHandler(tool, version, framework, removeCache, finalToolArgs);
                var existingPublishedTool = await toolHandler.CheckValidTool();
                if (existingPublishedTool)
                {
                    var result = await toolHandler.StartTool();
                    invocationContext.ExitCode = result;
                    return;
                }
                Console.WriteLine($"[X] The specified tool '{tool}' does not exist... " +
                                  $"Please check the correct name at https://www.nuget.org/packages");
                invocationContext.ExitCode = 1;
                return;
            }

            if (removeCache)
            {
                CacheManager.RemoveAllCachedFiles();
                return;
            }
            Console.WriteLine("[X] Please specify a tool or a valid option to work without tool...");
            invocationContext.ExitCode = 1;
        }
    }
}
