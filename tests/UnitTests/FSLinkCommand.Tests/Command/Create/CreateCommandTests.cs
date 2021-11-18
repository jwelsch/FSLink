using Autofac.Extras.NSubstitute;
using AutoFixture;
using FluentAssertions;
using FSLinkCommand.Command;
using FSLinkCommand.Command.Create;
using FSLinkCommon.Wraps;
using FSLinkLib;
using NSubstitute;
using System;
using Xunit;

namespace FSLinkCommand.Tests.Command.Create
{
    public class CreateCommandTests
    {
        private readonly static Fixture AutoFixture = new();
        private const string CommandName = "Create";

        [Fact]
        public void When_ctor_called_then_properties_are_as_expected()
        {
            using var autoSub = new AutoSubstitute();

            var sut = autoSub.Resolve<CreateCommand>();

            sut.Name.Should().Be(CommandName);
        }

        [Fact]
        public void When_hardlink_then_return_success()
        {
            using var autoSub = new AutoSubstitute();

            var linkPath = AutoFixture.Create<string>();
            var targetPath = AutoFixture.Create<string>();

            var fsLink = autoSub.Resolve<IFileSystemLink>();

            var arguments = autoSub.Resolve<ICreateArguments>();
            arguments.LinkType.Returns(FileSystemLinkType.HardLink);
            arguments.LinkPath.Returns(linkPath);
            arguments.TargetPath.Returns(targetPath);

            var sut = autoSub.Resolve<CreateCommand>();

            var task = sut.Run(arguments);
            task.Wait();

            task.Result.Should().BeOfType(typeof(SuccessCommandResult));
            task.Result.CommandName.Should().Be(CommandName);
            task.Result.Data.Should().BeOfType(typeof(string[]));
            task.Result.Error.Should().BeNull();

            var data = (string[])task.Result.Data;
            data.Should().HaveCount(2);
            data[0].Should().Be(linkPath);
            data[1].Should().Be(targetPath);

            fsLink.Received(1).CreateHardLink(linkPath, targetPath);
        }

        [Fact]
        public void When_junction_and_directory_exists_then_return_success()
        {
            using var autoSub = new AutoSubstitute();

            var linkPath = AutoFixture.Create<string>();
            var targetPath = AutoFixture.Create<string>();

            var fsLink = autoSub.Resolve<IFileSystemLink>();
            var directoryWrap = autoSub.Resolve<IDirectoryWrap>();
            directoryWrap.Exists(linkPath).Returns(true);

            var arguments = autoSub.Resolve<ICreateArguments>();
            arguments.LinkType.Returns(FileSystemLinkType.Junction);
            arguments.LinkPath.Returns(linkPath);
            arguments.TargetPath.Returns(targetPath);

            var sut = autoSub.Resolve<CreateCommand>();

            var task = sut.Run(arguments);
            task.Wait();

            task.Result.Should().BeOfType(typeof(SuccessCommandResult));
            task.Result.CommandName.Should().Be(CommandName);
            task.Result.Data.Should().BeOfType(typeof(string[]));
            task.Result.Error.Should().BeNull();

            var data = (string[])task.Result.Data;
            data.Should().HaveCount(2);
            data[0].Should().Be(linkPath);
            data[1].Should().Be(targetPath);

            directoryWrap.DidNotReceive().CreateDirectory(Arg.Any<string>());
            fsLink.Received(1).CreateJunction(linkPath, targetPath);
        }

        [Fact]
        public void When_junction_and_directory_does_not_exist_then_return_success()
        {
            using var autoSub = new AutoSubstitute();

            var linkPath = AutoFixture.Create<string>();
            var targetPath = AutoFixture.Create<string>();

            var fsLink = autoSub.Resolve<IFileSystemLink>();
            var directoryWrap = autoSub.Resolve<IDirectoryWrap>();
            directoryWrap.Exists(linkPath).Returns(false);

            var arguments = autoSub.Resolve<ICreateArguments>();
            arguments.LinkType.Returns(FileSystemLinkType.Junction);
            arguments.LinkPath.Returns(linkPath);
            arguments.TargetPath.Returns(targetPath);

            var sut = autoSub.Resolve<CreateCommand>();

            var task = sut.Run(arguments);
            task.Wait();

            task.Result.Should().BeOfType(typeof(SuccessCommandResult));
            task.Result.CommandName.Should().Be(CommandName);
            task.Result.Data.Should().BeOfType(typeof(string[]));
            task.Result.Error.Should().BeNull();

            var data = (string[])task.Result.Data;
            data.Should().HaveCount(2);
            data[0].Should().Be(linkPath);
            data[1].Should().Be(targetPath);

            directoryWrap.Received(1).CreateDirectory(linkPath);
            fsLink.Received(1).CreateJunction(linkPath, targetPath);
        }

        [Fact]
        public void When_symboliclink_and_directory_then_return_success()
        {
            using var autoSub = new AutoSubstitute();

            var linkPath = AutoFixture.Create<string>();
            var targetPath = AutoFixture.Create<string>();

            var fsLink = autoSub.Resolve<IFileSystemLink>();
            var fileWrap = autoSub.Resolve<IFileWrap>();
            fileWrap.GetAttributes(targetPath).Returns(System.IO.FileAttributes.Directory);

            var arguments = autoSub.Resolve<ICreateArguments>();
            arguments.LinkType.Returns(FileSystemLinkType.SymbolicLink);
            arguments.LinkPath.Returns(linkPath);
            arguments.TargetPath.Returns(targetPath);

            var sut = autoSub.Resolve<CreateCommand>();

            var task = sut.Run(arguments);
            task.Wait();

            task.Result.Should().BeOfType(typeof(SuccessCommandResult));
            task.Result.CommandName.Should().Be(CommandName);
            task.Result.Data.Should().BeOfType(typeof(string[]));
            task.Result.Error.Should().BeNull();

            var data = (string[])task.Result.Data;
            data.Should().HaveCount(2);
            data[0].Should().Be(linkPath);
            data[1].Should().Be(targetPath);

            fileWrap.Received(1).GetAttributes(targetPath);
            fsLink.Received(1).CreateSymbolicLink(linkPath, targetPath, SymbolicLinkType.Directory);
        }

        [Fact]
        public void When_symboliclink_and_file_then_return_success()
        {
            using var autoSub = new AutoSubstitute();

            var linkPath = AutoFixture.Create<string>();
            var targetPath = AutoFixture.Create<string>();

            var fsLink = autoSub.Resolve<IFileSystemLink>();
            var fileWrap = autoSub.Resolve<IFileWrap>();
            fileWrap.GetAttributes(targetPath).Returns(System.IO.FileAttributes.Normal);

            var arguments = autoSub.Resolve<ICreateArguments>();
            arguments.LinkType.Returns(FileSystemLinkType.SymbolicLink);
            arguments.LinkPath.Returns(linkPath);
            arguments.TargetPath.Returns(targetPath);

            var sut = autoSub.Resolve<CreateCommand>();

            var task = sut.Run(arguments);
            task.Wait();

            task.Result.Should().BeOfType(typeof(SuccessCommandResult));
            task.Result.CommandName.Should().Be(CommandName);
            task.Result.Data.Should().BeOfType(typeof(string[]));
            task.Result.Error.Should().BeNull();

            var data = (string[])task.Result.Data;
            data.Should().HaveCount(2);
            data[0].Should().Be(linkPath);
            data[1].Should().Be(targetPath);

            fileWrap.Received(1).GetAttributes(targetPath);
            fsLink.Received(1).CreateSymbolicLink(linkPath, targetPath, SymbolicLinkType.File);
        }

        [Fact]
        public void When_unknown_link_type_then_return_error()
        {
            using var autoSub = new AutoSubstitute();

            var arguments = autoSub.Resolve<ICreateArguments>();
            arguments.LinkType.Returns(FileSystemLinkType.None);

            var sut = autoSub.Resolve<CreateCommand>();

            var task = sut.Run(arguments);
            task.Wait();

            task.Result.Should().BeOfType(typeof(ErrorCommandResult));
            task.Result.CommandName.Should().Be(CommandName);
            task.Result.Data.Should().BeNull();
            task.Result.Error.Exception.Should().BeOfType(typeof(ArgumentException));
        }
    }
}
