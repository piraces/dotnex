using System;
using System.CommandLine;
using System.Threading.Tasks;

namespace dotnex
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
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

            rootCommand.SetHandler(async (string? tool, string version, string framework, bool removeCache, string[]? toolArgs) => {
                await Execute(tool, version, framework, removeCache, toolArgs);
            }, versionOption, frameworkOption, removeCacheOption, toolArgument, toolArgsArgument);

            return rootCommand.Invoke(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="version"></param>
        /// <param name="framework"></param>
        /// <param name="removeCache"></param>
        /// <param name="toolArgs"></param>
        /// <returns></returns>
        private static async Task<int> Execute(string? tool, string version, string framework, bool removeCache,
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
