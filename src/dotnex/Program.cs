using System;
using System.CommandLine;
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

            var feedOption = new Option<string>(
                    new[] { "--feed-directory", "-d" },
                    "Feed to retrieve dotnet tools from (defaults to Nuget.org)");

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
                feedOption,
                toolArgument,
                toolArgsArgument
            };

            rootCommand.SetHandler(async (string? tool, string version, string framework, string feed, bool removeCache, string[]? toolArgs) => {
                await Execute(tool, version, framework, feed, removeCache, toolArgs);
            }, versionOption, frameworkOption, feedOption, removeCacheOption, toolArgument, toolArgsArgument);

            return rootCommand.Invoke(args);
        }

        /// <summary>
        /// Main method that handles all the inputs and executes the external tool with the provided tool arguments and options.
        /// The external tool downloaded and executed follows the version and target framework specified.
        /// </summary>
        /// <param name="tool">The external dotnet tool to execute</param>
        /// <param name="version">The version of the dotnet tool to download and execute</param>
        /// <param name="framework">The target framework for the dotnet tool</param>
        /// <param name="feed">NuGet feed to fetch the dotnet tools from (default Nuget.org)</param>
        /// <param name="removeCache">If true, it clears the main cache of dotnex, where dotnet tools are stored for subsequent executions</param>
        /// <param name="toolArgs">Options and arguments to pass to the external dotnet tool</param>
        /// <returns>The exit code resulting from the execution of the external dotnet tool</returns>
        private static async Task<int> Execute(string? tool, string version, string framework, string feed, bool removeCache,
            string[]? toolArgs)
        {
            var finalToolArgs = toolArgs != null ? string.Join(' ', toolArgs) : null;
            if (!string.IsNullOrWhiteSpace(tool))
            {
                var toolHandler = new ToolHandler(tool, version, framework, removeCache, finalToolArgs);
                var existingPublishedTool = await toolHandler.CheckValidTool();
                if (existingPublishedTool)
                {
                    return await toolHandler.StartTool();
                }
                Console.WriteLine($"[X] The specified tool '{tool}' does not exist... " +
                                  $"Please check the correct name at https://www.nuget.org/packages");
                return 1;
            }

            if (removeCache)
            {
                return CacheManager.RemoveAllCachedFiles();
            }
            Console.WriteLine("[X] Please specify a tool or a valid option to work without tool...");
            return 1;
        }
    }
}
