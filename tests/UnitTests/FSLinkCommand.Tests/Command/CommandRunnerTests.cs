using Autofac.Extras.NSubstitute;
using AutoFixture;
using FluentAssertions;
using FSLinkCommand.Command;
using NSubstitute;
using System;
using Xunit;

namespace FSLinkCommand.Tests.Command
{
    public class CommandRunnerTests
    {
        private readonly static Fixture AutoFixture = new();

        [Fact]
        public void When_command_returns_success_then_success_result_is_returned()
        {
            using var autoSub = new AutoSubstitute();

            var success = AutoFixture.Create<SuccessCommandResult>();

            var arguments = autoSub.Resolve<ICommandArguments>();

            var command = autoSub.Resolve<ICommandBase>();
            command.Run(arguments).Returns(success);

            var sut = autoSub.Resolve<CommandRunner>();

            var result = sut.Execute(command, arguments);

            result.Should().Be(success);
        }

        [Fact]
        public void When_command_returns_error_then_error_result_is_returned()
        {
            using var autoSub = new AutoSubstitute();

            var error = AutoFixture.Create<ErrorCommandResult>();

            var arguments = autoSub.Resolve<ICommandArguments>();

            var command = autoSub.Resolve<ICommandBase>();
            command.Run(arguments).Returns(error);

            var sut = autoSub.Resolve<CommandRunner>();

            var result = sut.Execute(command, arguments);

            result.Should().Be(error);
        }

        [Fact]
        public void When_command_throws_exception_then_error_result_is_returned()
        {
            using var autoSub = new AutoSubstitute();

            var name = AutoFixture.Create<string>();

            var arguments = autoSub.Resolve<ICommandArguments>();

            var command = autoSub.Resolve<ICommandBase>();
            command.Name.Returns(name);
            command.When(x => x.Run(arguments))
                   .Do(x => throw new Exception());

            var sut = autoSub.Resolve<CommandRunner>();

            var result = sut.Execute(command, arguments);

            result.Should().BeOfType(typeof(ErrorCommandResult));
            result.CommandName.Should().Be(name);
            result.Error.Should().NotBeNull();
            result.Data.Should().BeNull();
        }
    }
}
