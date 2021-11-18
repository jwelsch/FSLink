using FSLinkLib;

namespace FSLinkCommand.FileSystem
{
    public interface IReparsePointLink
    {
        void Unlink(string linkPath);
    }

    public abstract class ReparsePointLink : IReparsePointLink
    {
        private readonly IFileSystemLink _fileSystemLink;

        protected ReparsePointLink(IFileSystemLink fileSystemLink)
        {
            _fileSystemLink = fileSystemLink;
        }

        public void Unlink(string linkPath)
        {
            _fileSystemLink.DeleteReparsePoint(linkPath);
        }
    }
}
