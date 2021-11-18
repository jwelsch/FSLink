using FSLinkLib;
using System;

#nullable enable

namespace FSLinkCommand.Command.Scan
{
    public interface IScanOutput
    {
        bool OnFileSystemEntry(string path, FileSystemLinkType linkType);

        bool OnFileSystemError(string path, Exception? error);
    }
}
