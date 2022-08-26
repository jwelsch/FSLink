using FSLinkLib;
using FSLinkLib.ReparsePoints;
using System;

#nullable enable

namespace FSLinkCommand.Command.Scan
{
    public interface IScanOutput
    {
        bool OnFileSystemEntry(string path, FileSystemLinkType linkType, IReparsePoint? reparsePoint);

        bool OnFileSystemError(string path, Exception? error);
    }
}
