using System;

namespace FSLinkLib.PInvoke
{
    [Flags]
    // dwDesiredAccess of CreateFile
    internal enum FileDesiredAccess : uint
    {
        GenericZero = 0,
        GenericRead = 0x80000000,
        GenericWrite = 0x40000000,
        GenericExecute = 0x20000000,
        GenericAll = 0x10000000,
    }

    [Flags]
    // dwShareMode of CreateFile
    internal enum FileShareMode : uint
    {
        None = 0x00000000,
        Read = 0x00000001,
        Write = 0x00000002,
        Delete = 0x00000004,
    }

    // dwCreationDisposition of CreateFile
    internal enum FileCreationDisposition : uint
    {
        New = 1,
        CreateAlways = 2,
        OpenExisting = 3,
        OpenAlways = 4,
        TruncateExisting = 5,
    }

    [Flags]
    // dwFlagsAndAttributes
    internal enum FileAttributes : uint
    {
        Readonly = 0x00000001,
        Hidden = 0x00000002,
        System = 0x00000004,
        Archive = 0x00000020,
        Encrypted = 0x00004000,
        Write_Through = 0x80000000,
        Overlapped = 0x40000000,
        NoBuffering = 0x20000000,
        RandomAccess = 0x10000000,
        SequentialScan = 0x08000000,
        DeleteOnClose = 0x04000000,
        BackupSemantics = 0x02000000,
        PosixSemantics = 0x01000000,
        OpenReparsePoint = 0x00200000,
        OpenNoRecall = 0x00100000,
        SessionAware = 0x00800000,
        Normal = 0x00000080
    }

    internal enum FINDEX_INFO_LEVELS : uint
    {
        FindExInfoStandard = 0x0u,
        FindExInfoBasic = 0x1u,
        FindExInfoMaxInfoLevel = 0x2u,
    }

    internal enum FINDEX_SEARCH_OPS : uint
    {
        FindExSearchNameMatch = 0x0u,
        FindExSearchLimitToDirectories = 0x1u,
        FindExSearchLimitToDevices = 0x2u,
        FindExSearchMaxSearchOp = 0x3u,
    }

    [Flags]
    internal enum SYMBOLIC_LINK_FLAG
    {
        File = 0,
        Directory = 1,
        AllowUnprivilegedCreate = 2
    }
}
