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
        private readonly IFileSystemPrivilege _fileSystemPrivilege;

        protected ReparsePointLink(IFileSystemLink fileSystemLink, IFileSystemPrivilege fileSystemPrivilege)
        {
            _fileSystemLink = fileSystemLink;
            _fileSystemPrivilege = fileSystemPrivilege;
        }

        public void Unlink(string linkPath)
        {
            _fileSystemPrivilege.EnsureRestoreName();

            _fileSystemLink.DeleteReparsePoint(linkPath);
        }
    }
}
