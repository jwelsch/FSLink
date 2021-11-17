using Autofac.Extras.NSubstitute;
using AutoFixture;
using FSLinkCommand.FileSystem;
using FSLinkLib;
using NSubstitute;
using Xunit;

namespace FSLinkCommand.Tests.FileSystem
{
    public class HardLinkTests
    {
        private readonly static Fixture AutoFixture = new();

        [Fact]
        public void When_create_is_called_then_correct_method_is_called()
        {
            using var autoSub = new AutoSubstitute();

            var linkPath = AutoFixture.Create<string>();
            var targetPath = AutoFixture.Create<string>();

            var fileSystemLink = autoSub.Resolve<IFileSystemLink>();

            var sut = autoSub.Resolve<HardLink>();

            sut.Create(linkPath, targetPath);

            fileSystemLink.Received(1).CreateHardLink(linkPath, targetPath);
        }

        [Fact]
        public void When_delete_is_called_then_correct_method_is_called()
        {
            using var autoSub = new AutoSubstitute();

            var linkPath = AutoFixture.Create<string>();

            var fileSystemLink = autoSub.Resolve<IFileSystemLink>();

            var sut = autoSub.Resolve<HardLink>();

            sut.Delete(linkPath);

            fileSystemLink.Received(1).DeleteHardLink(linkPath);
        }
    }
}
