using System.IO;

namespace FSLinkCommon.Wraps
{
    public interface IFileWrap
    {
        bool IsDirectory(string path);

        FileAttributes GetAttributes(string path);

        void Delete(string path);
    }

    public class FileWrap : IFileWrap
    {
        public bool IsDirectory(string path)
        {
            return (GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public FileAttributes GetAttributes(string path)
        {
            return File.GetAttributes(path);
        }

        public void Delete(string path)
        {
            File.Delete(path);
        }
    }
}
