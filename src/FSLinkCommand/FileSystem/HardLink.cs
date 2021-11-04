using FSLinkLib;

namespace FSLinkCommand.FileSystem
{
    public interface IHardLink
    {
        void Create(string hardLinkPath, string targetFilePath);

        void Delete(string hardLinkPath);
    }


    /// <summary>
    /// File system hard links that link two files.
    /// 
    /// The hard link path does not need to exist.
    /// The target path does not need to exist, but the hard link cannot be followed until it does.
    public class HardLink : IHardLink
    {
        private readonly IFileSystemLink _fileSystemLink;

        public HardLink(IFileSystemLink fileSystemLink)
        {
            _fileSystemLink = fileSystemLink;
        }

        public void Create(string hardLinkPath, string targetFilePath)
        {
            _fileSystemLink.CreateHardLink(hardLinkPath, targetFilePath);
        }

        public void Delete(string hardLinkPath)
        {
            _fileSystemLink.DeleteHardLink(hardLinkPath);
        }
    }
}
