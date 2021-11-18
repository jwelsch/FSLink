using Autofac.Extras.NSubstitute;
using AutoFixture;
using FluentAssertions;
using FSLinkCommand.Command;
using FSLinkCommand.Command.Delete;
using FSLinkCommon.Wraps;
using FSLinkLib;
using NSubstitute;
using System;
using Xunit;

namespace FSLinkCommand.Tests.Command.Delete
{
    public class DeleteCommandTests
    {
        private readonly static Fixture AutoFixture = new();
        private const string CommandName = "Delete";

        [Fact]
        public void When_ctor_called_then_properties_are_as_expected()
        {
            using var autoSub = new AutoSubstitute();

            var sut = autoSub.Resolve<DeleteCommand>();

            sut.Name.Should().Be(CommandName);
        }

        [Fact]
        public void When_file_then_delete_is_called()
        {
            using var autoSub = new AutoSubstitute();

            var path = AutoFixture.Create<string>();

            var fileWrap = autoSub.Resolve<IFileWrap>();
            fileWrap.IsDirectory(path).Returns(false);

            var arguments = autoSub.Resolve<IDeleteArguments>();
            arguments.Path.Returns(path);

            var sut = autoSub.Resolve<DeleteCommand>();

            var task = sut.Run(arguments);
            task.Wait();

            task.Result.Should().BeOfType(typeof(SuccessCommandResult));
            task.Result.CommandName.Should().Be(CommandName);
            task.Result.Data.Should().BeOfType(typeof(string));
            task.Result.Error.Should().BeNull();

            var data = (string)task.Result.Data;
            data.Should().Be(path);

            fileWrap.Received(1).IsDirectory(path);
            fileWrap.Received(1).Delete(path);
        }

        [Fact]
        public void When_directory_then_delete_is_called()
        {
            using var autoSub = new AutoSubstitute();

            var path = AutoFixture.Create<string>();

            var fileWrap = autoSub.Resolve<IFileWrap>();
            fileWrap.IsDirectory(path).Returns(true);

            var directoryWrap = autoSub.Resolve<IDirectoryWrap>();

            var arguments = autoSub.Resolve<IDeleteArguments>();
            arguments.Path.Returns(path);

            var sut = autoSub.Resolve<DeleteCommand>();

            var task = sut.Run(arguments);
            task.Wait();

            task.Result.Should().BeOfType(typeof(SuccessCommandResult));
            task.Result.CommandName.Should().Be(CommandName);
            task.Result.Data.Should().BeOfType(typeof(string));
            task.Result.Error.Should().BeNull();

            var data = (string)task.Result.Data;
            data.Should().Be(path);

            fileWrap.Received(1).IsDirectory(path);
            directoryWrap.Received(1).Delete(path);
        }
    }
}
