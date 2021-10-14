using System.Collections.Generic;
using System.IO;

namespace FSLinkCommon.Wraps
{
    public interface IDirectoryWrap
    {
        IEnumerable<string> EnumerateDirectories(string path, string searchPattern, EnumerationOptions enumerationOptions);

        string[] GetDirectories(string path, string searchPattern, EnumerationOptions enumerationOptions);

        IEnumerable<string> EnumerateFiles(string path, string searchPattern, EnumerationOptions enumerationOptions);

        string[] GetFiles(string path, string searchPattern, EnumerationOptions enumerationOptions);

        bool Exists(string path);

        IDirectoryInfoWrap CreateDirectory(string path);

        void Delete(string path, bool recursive = false);
    }

    public class DirectoryWrap : IDirectoryWrap
    {
        public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, EnumerationOptions enumerationOptions)
        {
            return Directory.EnumerateDirectories(path, searchPattern, enumerationOptions);
        }

        public string[] GetDirectories(string path, string searchPattern, EnumerationOptions enumerationOptions)
        {
            return Directory.GetDirectories(path, searchPattern, enumerationOptions);
        }

        public IEnumerable<string> EnumerateFiles(string path, string searchPattern, EnumerationOptions enumerationOptions)
        {
            return Directory.EnumerateFiles(path, searchPattern, enumerationOptions);
        }

        public string[] GetFiles(string path, string searchPattern, EnumerationOptions enumerationOptions)
        {
            return Directory.GetFiles(path, searchPattern, enumerationOptions);
        }

        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public IDirectoryInfoWrap CreateDirectory(string path)
        {
            return new DirectoryInfoWrap(Directory.CreateDirectory(path));
        }

        public void Delete(string path, bool recursive = false)
        {
            Directory.Delete(path, recursive);
        }
    }
}
