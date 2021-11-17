using Autofac.Extras.NSubstitute;
using FluentAssertions;
using FSLinkCommand.Command;
using FSLinkCommand.Command.Create;
using FSLinkCommand.Command.Delete;
using FSLinkCommand.Command.Relink;
using FSLinkCommand.Command.Reparse;
using FSLinkCommand.Command.Scan;
using System;
using Xunit;

namespace FSLinkCommand.Tests
{
    public class CommandFactoryTests
    {
        [Fact]
        public void When_unknown_commandarguments_given_then_throw()
        {
            using var autoSub = new AutoSubstitute();

            var arg = autoSub.Resolve<ICommandArguments>();

            var sut = autoSub.Resolve<CommandFactory>();

            Action act = () => sut.Create(arg);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void When_scanarguments_given_then_return_scancommand()
        {
            using var autoSub = new AutoSubstitute();

            var arg = autoSub.Resolve<IScanArguments>();

            var sut = autoSub.Resolve<CommandFactory>();

            var result = sut.Create(arg);

            result.Should().BeOfType(typeof(ScanCommand));
        }

        [Fact]
        public void When_createarguments_given_then_return_createcommand()
        {
            using var autoSub = new AutoSubstitute();

            var arg = autoSub.Resolve<ICreateArguments>();

            var sut = autoSub.Resolve<CommandFactory>();

            var result = sut.Create(arg);

            result.Should().BeOfType(typeof(CreateCommand));
        }

        [Fact]
        public void When_deletearguments_given_then_return_deletecommand()
        {
            using var autoSub = new AutoSubstitute();

            var arg = autoSub.Resolve<IDeleteArguments>();

            var sut = autoSub.Resolve<CommandFactory>();

            var result = sut.Create(arg);

            result.Should().BeOfType(typeof(DeleteCommand));
        }

        [Fact]
        public void When_relinkarguments_given_then_return_relinkcommand()
        {
            using var autoSub = new AutoSubstitute();

            var arg = autoSub.Resolve<IRelinkArguments>();

            var sut = autoSub.Resolve<CommandFactory>();

            var result = sut.Create(arg);

            result.Should().BeOfType(typeof(RelinkCommand));
        }

        [Fact]
        public void When_reparsearguments_given_then_return_reparsecommand()
        {
            using var autoSub = new AutoSubstitute();

            var arg = autoSub.Resolve<IReparseArguments>();

            var sut = autoSub.Resolve<CommandFactory>();

            var result = sut.Create(arg);

            result.Should().BeOfType(typeof(ReparseCommand));
        }
    }
}
