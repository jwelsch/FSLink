using FSLinkCommon.Wraps;
using System;
using System.IO;

namespace FSLinkCommand.Scan
{
    public interface IFileSystemScanner
    {
        void ScanPath(string path, string searchPattern, SearchOption searchOption, Func<string, bool> callback);
    }

    public class FileSystemScanner : IFileSystemScanner
    {
        private readonly IDirectoryWrap _directoryWrap;
        private readonly IFileWrap _fileWrap;

        public FileSystemScanner(IDirectoryWrap directoryWrap, IFileWrap fileWrap)
        {
            _directoryWrap = directoryWrap;
            _fileWrap = fileWrap;
        }

        public void ScanPath(string path, string searchPattern, SearchOption searchOption, Func<string, bool> callback)
        {
            var enumerationOptions = new EnumerationOptions
            {
                AttributesToSkip = 0,
                MatchCasing = MatchCasing.CaseInsensitive,
                MatchType = MatchType.Simple,
                RecurseSubdirectories = searchOption == SearchOption.AllDirectories
            };

            ScanPath(path, searchPattern, enumerationOptions, callback);
        }

        private bool ScanPath(string path, string searchPattern, EnumerationOptions enumerationOptions, Func<string, bool> callback)
        {
            var attributes = _fileWrap.GetAttributes(path);
            if ((attributes & FileAttributes.Directory) != FileAttributes.Directory)
            {
                return callback(path);
            }

            var filePathEnumerator = _directoryWrap.EnumerateFiles(path, searchPattern, enumerationOptions);

            foreach (var filePath in filePathEnumerator)
            {
                var result = callback(filePath);

                if (!result)
                {
                    return false;
                }
            }

            var directoryPathEnumerator = _directoryWrap.EnumerateDirectories(path, searchPattern, enumerationOptions);

            foreach (var directoryPath in directoryPathEnumerator)
            {
                if (!callback(directoryPath))
                {
                    return false;
                }

                if (enumerationOptions.RecurseSubdirectories)
                {
                    if (!ScanPath(directoryPath, searchPattern, enumerationOptions, callback))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
