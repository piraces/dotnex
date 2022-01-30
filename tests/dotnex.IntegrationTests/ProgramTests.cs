using FluentAssertions;
using System;
using Xunit;

namespace dotnex.IntegrationTests
{
    public class ProgramTests
    {
        [Fact]
        public void ProgramShouldFailIfNoToolAndNoValidOptionSpecified()
        {
            var result = Program.Main(new string[] { });
            result.Should().Be(1);
        }

        [Fact]
        public void ProgramShouldFailIfNoExistingToolSpecified()
        {
            var result = Program.Main(new[] { Guid.NewGuid().ToString() });
            result.Should().Be(1);
        }

        [Fact]
        public void ProgramShouldSuccessIfNoToolAndValidOptionSpecified()
        {
            var result = Program.Main(new[] { "--remove-cache" });
            result.Should().Be(0);
        }
        
        [Fact]
        public void ProgramShouldSuccessIfValidToolSpecified()
        {
            var result = Program.Main(new[] { "dotnetsay" });
            result.Should().Be(0);
        }
    }
}
