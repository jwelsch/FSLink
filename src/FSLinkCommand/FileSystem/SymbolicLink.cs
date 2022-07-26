using FSLinkCommon.Wraps;
using FSLinkLib;

namespace FSLinkCommand.FileSystem
{
    public interface ISymbolicLink : IReparsePointLink
    {
        void Create(string symbolicLinkPath, string targetPath);

        void Delete(string symbolicLinkPath);
    }

    /// <summary>
    /// File system symbolic links that use reparse points. They can either be file-to-file or directory-to-directory links.
    /// 
    /// The symbolic link path does not need to exist.
    /// The target path does not need to exist, but the symbolic link cannot be followed until it does.
    /// </summary>
    public class SymbolicLink : ReparsePointLink, ISymbolicLink
    {
        private readonly IFileSystemLink _fileSystemLink;
        private readonly IFileSystemPrivilege _fileSystemPrivilege;
        private readonly IFileWrap _fileWrap;

        public SymbolicLink(IFileSystemLink fileSystemLink, IFileSystemPrivilege fileSystemPrivilege, IFileWrap fileWrap)
            : base(fileSystemLink, fileSystemPrivilege)
        {
            _fileSystemLink = fileSystemLink;
            _fileSystemPrivilege = fileSystemPrivilege;
            _fileWrap = fileWrap;
        }

        public void Create(string symbolicLinkPath, string targetPath)
        {
            var symbolicLinkType = _fileWrap.IsDirectory(targetPath) ? SymbolicLinkType.Directory : SymbolicLinkType.File;

            _fileSystemLink.CreateSymbolicLink(symbolicLinkPath, targetPath, symbolicLinkType);
        }

        public void Delete(string symbolicLinkPath)
        {
            _fileSystemLink.DeleteSymbolicLink(symbolicLinkPath);
        }
    }
}
