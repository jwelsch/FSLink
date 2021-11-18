using Autofac.Extras.NSubstitute;
using AutoFixture;
using FluentAssertions;
using FSLinkCommand.Command;
using FSLinkCommand.Command.Relink;
using FSLinkCommand.FileSystem;
using FSLinkLib;
using NSubstitute;
using Xunit;

namespace FSLinkCommand.Tests.Command.Relink
{
    public class RelinkCommandTests
    {
        private readonly static Fixture AutoFixture = new();
        private const string CommandName = "Relink";

        [Fact]
        public void When_ctor_called_then_properties_are_as_expected()
        {
            using var autoSub = new AutoSubstitute();

            var sut = autoSub.Resolve<RelinkCommand>();

            sut.Name.Should().Be(CommandName);
        }

        [Fact]
        public void When_hardlink_then_return_success()
        {
            using var autoSub = new AutoSubstitute();

            var linkPath = AutoFixture.Create<string>();
            var newTargetPath = AutoFixture.Create<string>();

            var hardLink = autoSub.Resolve<IHardLink>();

            var fsLink = autoSub.Resolve<IFileSystemLink>();
            fsLink.GetLinkType(linkPath).Returns(FileSystemLinkType.HardLink);

            var arguments = autoSub.Resolve<IRelinkArguments>();
            arguments.LinkPath.Returns(linkPath);
            arguments.NewTargetPath.Returns(newTargetPath);

            var sut = autoSub.Resolve<RelinkCommand>();

            var task = sut.Run(arguments);
            task.Wait();

            task.Result.Should().BeOfType(typeof(SuccessCommandResult));
            task.Result.CommandName.Should().Be(CommandName);
            task.Result.Data.Should().BeOfType(typeof(string[]));
            task.Result.Error.Should().BeNull();

            var data = (string[])task.Result.Data;
            data.Should().HaveCount(2);
            data[0].Should().Be(linkPath);
            data[1].Should().Be(newTargetPath);

            fsLink.Received(1).GetLinkType(linkPath);
            hardLink.Received(1).Delete(linkPath);
            hardLink.Received(1).Create(linkPath, newTargetPath);
        }

        [Fact]
        public void When_junction_then_return_success()
        {
            using var autoSub = new AutoSubstitute();

            var linkPath = AutoFixture.Create<string>();
            var newTargetPath = AutoFixture.Create<string>();

            var junction = autoSub.Resolve<IJunction>();

            var fsLink = autoSub.Resolve<IFileSystemLink>();
            fsLink.GetLinkType(linkPath).Returns(FileSystemLinkType.Junction);

            var arguments = autoSub.Resolve<IRelinkArguments>();
            arguments.LinkPath.Returns(linkPath);
            arguments.NewTargetPath.Returns(newTargetPath);

            var sut = autoSub.Resolve<RelinkCommand>();

            var task = sut.Run(arguments);
            task.Wait();

            task.Result.Should().BeOfType(typeof(SuccessCommandResult));
            task.Result.CommandName.Should().Be(CommandName);
            task.Result.Data.Should().BeOfType(typeof(string[]));
            task.Result.Error.Should().BeNull();

            var data = (string[])task.Result.Data;
            data.Should().HaveCount(2);
            data[0].Should().Be(linkPath);
            data[1].Should().Be(newTargetPath);

            fsLink.Received(1).GetLinkType(linkPath);
            junction.Received(1).Unlink(linkPath);
            junction.Received(1).Create(linkPath, newTargetPath);
        }

        [Fact]
        public void When_symboliclink_then_return_success()
        {
            using var autoSub = new AutoSubstitute();

            var linkPath = AutoFixture.Create<string>();
            var newTargetPath = AutoFixture.Create<string>();

            var symbolicLink = autoSub.Resolve<ISymbolicLink>();

            var fsLink = autoSub.Resolve<IFileSystemLink>();
            fsLink.GetLinkType(linkPath).Returns(FileSystemLinkType.SymbolicLink);

            var arguments = autoSub.Resolve<IRelinkArguments>();
            arguments.LinkPath.Returns(linkPath);
            arguments.NewTargetPath.Returns(newTargetPath);

            var sut = autoSub.Resolve<RelinkCommand>();

            var task = sut.Run(arguments);
            task.Wait();

            task.Result.Should().BeOfType(typeof(SuccessCommandResult));
            task.Result.CommandName.Should().Be(CommandName);
            task.Result.Data.Should().BeOfType(typeof(string[]));
            task.Result.Error.Should().BeNull();

            var data = (string[])task.Result.Data;
            data.Should().HaveCount(2);
            data[0].Should().Be(linkPath);
            data[1].Should().Be(newTargetPath);

            fsLink.Received(1).GetLinkType(linkPath);
            symbolicLink.Received(1).Unlink(linkPath);
            symbolicLink.Received(1).Create(linkPath, newTargetPath);
        }
    }
}
