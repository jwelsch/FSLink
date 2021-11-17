using Autofac.Extras.NSubstitute;
using AutoFixture;
using FSLinkCommand.FileSystem;
using FSLinkLib;
using NSubstitute;
using Xunit;

namespace FSLinkCommand.Tests.FileSystem
{
    public class JunctionTests
    {
        private readonly static Fixture AutoFixture = new();

        [Fact]
        public void When_create_is_called_then_correct_method_is_called()
        {
            using var autoSub = new AutoSubstitute();

            var linkPath = AutoFixture.Create<string>();
            var targetPath = AutoFixture.Create<string>();

            var fileSystemLink = autoSub.Resolve<IFileSystemLink>();

            var sut = autoSub.Resolve<Junction>();

            sut.Create(linkPath, targetPath);

            fileSystemLink.Received(1).CreateJunction(linkPath, targetPath);
        }

        [Fact]
        public void When_delete_is_called_then_correct_method_is_called()
        {
            using var autoSub = new AutoSubstitute();

            var linkPath = AutoFixture.Create<string>();

            var fileSystemLink = autoSub.Resolve<IFileSystemLink>();

            var sut = autoSub.Resolve<Junction>();

            sut.Delete(linkPath);

            fileSystemLink.Received(1).DeleteJunction(linkPath);
        }
    }
}
