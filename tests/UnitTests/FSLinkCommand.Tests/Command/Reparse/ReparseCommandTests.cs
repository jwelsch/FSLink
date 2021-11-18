using Autofac.Extras.NSubstitute;
using AutoFixture;
using FluentAssertions;
using FSLinkCommand.Command;
using FSLinkCommand.Command.Reparse;
using FSLinkLib;
using FSLinkLib.ReparsePoints;
using NSubstitute;
using System.IO;
using Xunit;

#nullable enable

namespace FSLinkCommand.Tests.Command.Relink
{
    public class ReparseCommandTests
    {
        private readonly static Fixture AutoFixture = new();
        private const string CommandName = "Reparse";

        [Fact]
        public void When_ctor_called_then_properties_are_as_expected()
        {
            using var autoSub = new AutoSubstitute();

            var sut = autoSub.Resolve<ReparseCommand>();

            sut.Name.Should().Be(CommandName);
        }

        [Fact]
        public void When_reparse_point_given_then_return_success()
        {
            using var autoSub = new AutoSubstitute();

            var path = AutoFixture.Create<string>();

            var arguments = autoSub.Resolve<IReparseArguments>();
            arguments.Path.Returns(path);

            var reparsePoint = autoSub.Resolve<IReparsePoint>();

            var reparseOutput = autoSub.Resolve<IReparseOutput>();

            var logReparseOutputFactory = autoSub.Resolve<ILogReparseOutputFactory>();
            logReparseOutputFactory.Create(reparsePoint).Returns(reparseOutput);

            var fsLink = autoSub.Resolve<IFileSystemLink>();
            fsLink.GetReparsePoint(path).Returns(reparsePoint);

            var sut = autoSub.Resolve<ReparseCommand>();

            var task = sut.Run(arguments);
            task.Wait();

            task.Result.Should().BeOfType(typeof(SuccessCommandResult));
            task.Result.CommandName.Should().Be(CommandName);
            task.Result.Data.Should().Be(reparsePoint);
            task.Result.Error.Should().BeNull();

            fsLink.Received(1).GetReparsePoint(path);
            logReparseOutputFactory.Received(1).Create(reparsePoint);
            reparseOutput.Received(1).OnReparsePointData(reparsePoint);
        }


        [Fact]
        public void When_reparse_point_not_given_then_return_error()
        {
            using var autoSub = new AutoSubstitute();

            var path = AutoFixture.Create<string>();

            var arguments = autoSub.Resolve<IReparseArguments>();
            arguments.Path.Returns(path);

            var reparsePoint = autoSub.Resolve<IReparsePoint>();

            var reparseOutput = autoSub.Resolve<IReparseOutput>();

            var logReparseOutputFactory = autoSub.Resolve<ILogReparseOutputFactory>();
            logReparseOutputFactory.Create(reparsePoint).Returns(reparseOutput);

            var fsLink = autoSub.Resolve<IFileSystemLink>();
            fsLink.GetReparsePoint(path).Returns((IReparsePoint?)null);

            var sut = autoSub.Resolve<ReparseCommand>();

            var task = sut.Run(arguments);
            task.Wait();

            task.Result.Should().BeOfType(typeof(ErrorCommandResult));
            task.Result.CommandName.Should().Be(CommandName);
            task.Result.Data.Should().BeNull();
            task.Result.Error.Should().NotBeNull();
            task.Result.Error!.Code.Should().BeNull();
            task.Result.Error!.Message.Should().BeNull();
            task.Result.Error!.Exception.Should().BeOfType(typeof(IOException));

            fsLink.Received(1).GetReparsePoint(path);
            logReparseOutputFactory.DidNotReceive().Create(Arg.Any<IReparsePoint>());
            reparseOutput.DidNotReceive().OnReparsePointData(Arg.Any<IReparsePoint>());
        }
    }
}
