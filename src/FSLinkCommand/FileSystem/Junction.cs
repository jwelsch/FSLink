using FSLinkLib;

namespace FSLinkCommand.FileSystem
{
    public interface IJunction : IReparsePointLink
    {
        void Create(string junctionPath, string targetDirectoryPath);

        void Delete(string junctionPath);
    }

    /// <summary>
    /// File system junctions that use reparse points to link two directories.
    /// 
    /// The junction path does not need to exist.
    /// The target path does not need to exist, but the junction cannot be followed until it does.
    /// </summary>
    public class Junction : ReparsePointLink, IJunction
    {
        private readonly IFileSystemLink _fileSystemLink;

        public Junction(IFileSystemLink fileSystemLink)
            : base(fileSystemLink)
        {
            _fileSystemLink = fileSystemLink;
        }

        public void Create(string junctionPath, string targetDirectoryPath)
        {
            _fileSystemLink.CreateJunction(junctionPath, targetDirectoryPath);
        }

        public void Delete(string junctionPath)
        {
            _fileSystemLink.DeleteJunction(junctionPath);
        }
    }
}
