using FSLinkLib;

namespace FSLinkCommand.FileSystem
{
    public interface IReparsePointLink
    {
        void Unlink(string symbolicLinkPath);
    }

    public abstract class ReparsePointLink : IReparsePointLink
    {
        private readonly IFileSystemLink _fileSystemLink;

        protected ReparsePointLink(IFileSystemLink fileSystemLink)
        {
            _fileSystemLink = fileSystemLink;
        }

        public void Unlink(string symbolicLinkPath)
        {
            _fileSystemLink.DeleteReparsePoint(symbolicLinkPath);
        }
    }
}
