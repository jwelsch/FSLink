using Autofac.Extras.NSubstitute;
using AutoFixture;
using FluentAssertions;
using FSLinkCommand.Command;
using FSLinkCommand.Command.Scan;
using NSubstitute;
using System;
using System.IO;
using Xunit;

namespace FSLinkCommand.Tests.Command.Scan
{
    public class ScanCommandTests
    {
        private readonly static Fixture AutoFixture = new();
        private const string CommandName = "Scan";

        [Fact]
        public void When_ctor_called_then_properties_are_as_expected()
        {
            using var autoSub = new AutoSubstitute();

            var sut = autoSub.Resolve<ScanCommand>();

            sut.Name.Should().Be(CommandName);
        }

        [Fact]
        public void When_no_errors_then_return_success()
        {
            using var autoSub = new AutoSubstitute();

            var path = AutoFixture.Create<string>();

            var fsScanner = autoSub.Resolve<IFileSystemScanner>();

            var arguments = autoSub.Resolve<IScanArguments>();
            arguments.Path.Returns(path);
            arguments.Recurse.Returns(true);

            var sut = autoSub.Resolve<ScanCommand>();

            var task = sut.Run(arguments);
            task.Wait();

            task.Result.Should().BeOfType(typeof(SuccessCommandResult));
            task.Result.CommandName.Should().Be(CommandName);
            task.Result.Data.Should().BeOfType(typeof(Exception[]));
            task.Result.Error.Should().BeNull();

            var data = (Exception[])task.Result.Data;
            data.Should().HaveCount(0);

            fsScanner.Received(1).ScanPath(path, "*", SearchOption.AllDirectories, Arg.Any<Func<string, bool>>());
        }
    }
}
