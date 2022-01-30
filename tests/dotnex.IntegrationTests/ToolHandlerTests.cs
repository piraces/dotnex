using FluentAssertions;
using System;
using Xunit;

namespace dotnex.IntegrationTests
{
    public class ToolHandlerTests
    {

        [Fact]
        public async void ToolHandlerShouldRunValidToolWithSuccess()
        {
            var toolHandler = new ToolHandler("dotnetsay");
            var result = await toolHandler.StartTool();
            result.Should().Be(0);
        }

        [Fact]
        public async void ToolHandlerShouldRunValidToolWithArgumentsWithSuccess()
        {
            var toolHandler = new ToolHandler("dotnetsay", toolArgs: "Hello!");
            var result = await toolHandler.StartTool();
            result.Should().Be(0);
        }

        [Fact]
        public async void ToolHandlerShouldRunValidToolWithSpecifiedVersionWithSuccess()
        {
            var toolHandler = new ToolHandler("dotnetsay", version: "2.0.0");
            var result = await toolHandler.StartTool();
            result.Should().Be(0);
        }

        [Fact]
        public async void ToolHandlerShouldRunValidToolWithSpecifiedVersionAndTargetFrameworkWithSuccess()
        {
            var toolHandler = new ToolHandler("dotnetsay", version: "2.1.7", framework: "net6.0");
            var result = await toolHandler.StartTool();
            result.Should().Be(0);
        }

        [Fact]
        public async void ToolHandlerShouldVerifyIfToolIsNotValidAndReturnError()
        {
            var toolHandler = new ToolHandler(Guid.NewGuid().ToString());
            var isValidTool = await toolHandler.CheckValidTool();
            isValidTool.Should().Be(false);
        }

        [Fact]
        public async void ToolHandlerShouldVerifyIfToolIsValidAndReturnSuccess()
        {
            var toolHandler = new ToolHandler("dotnex");
            var isValidTool = await toolHandler.CheckValidTool();
            isValidTool.Should().Be(true);
        }
    }
}
