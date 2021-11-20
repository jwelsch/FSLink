using Autofac.Extras.NSubstitute;
using AutoFixture;
using FluentAssertions;
using FSLinkCommand.Command;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

#nullable enable

namespace FSLink.Tests
{
    public class ApplicationHostTests
    {
        private readonly static Fixture AutoFixture = new();

        [Fact]
        public void When_command_returns_sucess_then_return_zero()
        {
            var commandName = AutoFixture.Create<string>();

            using var autoSub = new AutoSubstitute();

            var logger = autoSub.Resolve<ILogger<ApplicationHost>>();
            var command = autoSub.Resolve<ICommandBase>();
            var arguments = autoSub.Resolve<ICommandArguments>();
            var commandResult = autoSub.Resolve<ICommandResult>();
            commandResult.Success.Returns(true);
            commandResult.CommandName.Returns(commandName);
            var runner = autoSub.Resolve<ICommandRunner>();
            runner.Execute(command, arguments).Returns(commandResult);

            var sut = autoSub.Resolve<ApplicationHost>();

            var result = sut.Run(command, arguments);

            result.Should().Be(0);
            runner.Received(1).Execute(command, arguments);
            logger.Received(1).LogInformation($"The {commandName} command has completed successfully.");
        }

        [Fact]
        public void When_command_returns_not_sucess_then_return_error_code_if_result_has_error_code()
        {
            var commandName = AutoFixture.Create<string>();
            var commandErrorCode = AutoFixture.Create<int>();

            using var autoSub = new AutoSubstitute();

            var logger = autoSub.Resolve<ILogger<ApplicationHost>>();
            var command = autoSub.Resolve<ICommandBase>();
            var arguments = autoSub.Resolve<ICommandArguments>();
            var commandError = autoSub.Resolve<ICommandError>();
            commandError.Code.Returns(commandErrorCode);
            var commandResult = autoSub.Resolve<ICommandResult>();
            commandResult.Success.Returns(false);
            commandResult.Error.Returns(commandError);
            commandResult.CommandName.Returns(commandName);
            var runner = autoSub.Resolve<ICommandRunner>();
            runner.Execute(command, arguments).Returns(commandResult);

            var sut = autoSub.Resolve<ApplicationHost>();

            var result = sut.Run(command, arguments);

            result.Should().Be(commandErrorCode);
            runner.Received(1).Execute(command, arguments);
            //logger.Received(1).LogError(Arg.Any<string>(), Arg.Any<object[]>());
        }

        [Fact]
        public void When_command_returns_not_sucess_then_return_int_minvalue_if_result_has_null_error_code()
        {
            var commandName = AutoFixture.Create<string>();

            using var autoSub = new AutoSubstitute();

            var logger = autoSub.Resolve<ILogger<ApplicationHost>>();
            var command = autoSub.Resolve<ICommandBase>();
            var arguments = autoSub.Resolve<ICommandArguments>();
            var commandError = autoSub.Resolve<ICommandError>();
            commandError.Code.Returns((int?)null);
            var commandResult = autoSub.Resolve<ICommandResult>();
            commandResult.Success.Returns(false);
            commandResult.Error.Returns(commandError);
            commandResult.CommandName.Returns(commandName);
            var runner = autoSub.Resolve<ICommandRunner>();
            runner.Execute(command, arguments).Returns(commandResult);

            var sut = autoSub.Resolve<ApplicationHost>();

            var result = sut.Run(command, arguments);

            result.Should().Be(int.MinValue);
            runner.Received(1).Execute(command, arguments);
        }

        [Fact]
        public void When_command_returns_not_sucess_then_return_int_minvalue_if_result_has_null_error()
        {
            var commandName = AutoFixture.Create<string>();

            using var autoSub = new AutoSubstitute();

            var logger = autoSub.Resolve<ILogger<ApplicationHost>>();
            var command = autoSub.Resolve<ICommandBase>();
            var arguments = autoSub.Resolve<ICommandArguments>();
            var commandResult = autoSub.Resolve<ICommandResult>();
            commandResult.Success.Returns(false);
            commandResult.Error.Returns((ICommandError?)null);
            commandResult.CommandName.Returns(commandName);
            var runner = autoSub.Resolve<ICommandRunner>();
            runner.Execute(command, arguments).Returns(commandResult);

            var sut = autoSub.Resolve<ApplicationHost>();

            var result = sut.Run(command, arguments);

            result.Should().Be(int.MinValue);
            runner.Received(1).Execute(command, arguments);
        }
    }
}
