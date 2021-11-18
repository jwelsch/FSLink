using Autofac.Extras.NSubstitute;
using AutoFixture;
using FSLinkCommand.FileSystem;
using FSLinkCommon.Wraps;
using FSLinkLib;
using NSubstitute;
using Xunit;

namespace FSLinkCommand.Tests.FileSystem
{
    public class SymbolicLinkTests
    {
        private readonly static Fixture AutoFixture = new();

        [Fact]
        public void When_create_is_called_with_file_target_then_correct_method_is_called()
        {
            using var autoSub = new AutoSubstitute();

            var linkPath = AutoFixture.Create<string>();
            var targetPath = AutoFixture.Create<string>();

            var fileSystemLink = autoSub.Resolve<IFileSystemLink>();
            var fileWrap = autoSub.Resolve<IFileWrap>();
            fileWrap.IsDirectory(targetPath).Returns(false);

            var sut = autoSub.Resolve<SymbolicLink>();

            sut.Create(linkPath, targetPath);

            fileSystemLink.Received(1).CreateSymbolicLink(linkPath, targetPath, SymbolicLinkType.File);
        }

        [Fact]
        public void When_create_is_called_with_directory_target_then_correct_method_is_called()
        {
            using var autoSub = new AutoSubstitute();

            var linkPath = AutoFixture.Create<string>();
            var targetPath = AutoFixture.Create<string>();

            var fileSystemLink = autoSub.Resolve<IFileSystemLink>();
            var fileWrap = autoSub.Resolve<IFileWrap>();
            fileWrap.IsDirectory(targetPath).Returns(true);

            var sut = autoSub.Resolve<SymbolicLink>();

            sut.Create(linkPath, targetPath);

            fileSystemLink.Received(1).CreateSymbolicLink(linkPath, targetPath, SymbolicLinkType.Directory);
        }

        [Fact]
        public void When_delete_is_called_then_correct_method_is_called()
        {
            using var autoSub = new AutoSubstitute();

            var linkPath = AutoFixture.Create<string>();

            var fileSystemLink = autoSub.Resolve<IFileSystemLink>();

            var sut = autoSub.Resolve<SymbolicLink>();

            sut.Delete(linkPath);

            fileSystemLink.Received(1).DeleteSymbolicLink(linkPath);
        }

        [Fact]
        public void When_unlink_is_called_then_correct_method_is_called()
        {
            using var autoSub = new AutoSubstitute();

            var linkPath = AutoFixture.Create<string>();

            var fileSystemLink = autoSub.Resolve<IFileSystemLink>();

            var sut = autoSub.Resolve<SymbolicLink>();

            sut.Unlink(linkPath);

            fileSystemLink.Received(1).DeleteReparsePoint(linkPath);
        }
    }
}
