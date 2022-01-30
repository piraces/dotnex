using FluentAssertions;
using Xunit;

namespace dotnex.IntegrationTests
{
    public class CliCommandWrapperTests
    {
        [Fact]
        public async void CliCommandWrapperShouldReturnCorrectExitCodeOnSuccess()
        {
            var cliCommandWrapper = new CliCommandLineWrapper(new string[] { "help" });
            var result = await cliCommandWrapper.StartCliCommand();
            result.Should().Be(0);
        }

        [Fact]
        public async void CliCommandWrapperShouldReturnCorrectExitCodeOnFail()
        {
            var cliCommandWrapper = new CliCommandLineWrapper(new string[] { "error" });
            var result = await cliCommandWrapper.StartCliCommand();
            result.Should().Be(1);
        }
    }
}
