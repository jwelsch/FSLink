using FSLinkLib;
using System;

#nullable enable

namespace FSLinkCommand.Scan
{
    public interface IScanOutput
    {
        void OnFileSystemEntry(string path, FileSystemLinkType linkType);

        void OnFileSystemError(string path, Exception? error);
    }
}
