﻿using System.Runtime.InteropServices;

namespace FSLinkLib.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct REPARSE_DATA_BUFFER_SYMBOLICLINK
    {
        public uint ReparseTag;
        public ushort ReparseDataLength;
        public ushort Reserved;
        public ushort SubstituteNameOffset;
        public ushort SubstituteNameLength;
        public ushort PrintNameOffset;
        public ushort PrintNameLength;
        public uint Flags;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3FF4)] // 16 KB - 12 bytes
        public byte[] PathBuffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct REPARSE_DATA_BUFFER_MOUNTPOINT
    {
        public uint ReparseTag;
        public ushort ReparseDataLength;
        public ushort Reserved;
        public ushort SubstituteNameOffset;
        public ushort SubstituteNameLength;
        public ushort PrintNameOffset;
        public ushort PrintNameLength;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3FF8)] // 16 KB - 8 bytes
        public byte[] PathBuffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BY_HANDLE_FILE_INFORMATION
    {
        public uint FileAttributes;
        public System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWriteTime;
        public uint VolumeSerialNumber;
        public uint FileSizeHigh;
        public uint FileSizeLow;
        public uint NumberOfLinks;
        public uint FileIndexHigh;
        public uint FileIndexLow;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GUID
    {
        public uint Data1;
        public ushort Data2;
        public ushort Data3;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] Data4;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WIN32_FIND_DATA
    {
        public uint dwFileAttributes;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
        public uint dwReserved0;
        public uint dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string cAlternateFileName;
    }

    /// <summary>
    /// Maximum size is 16 Kb.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct REPARSE_DATA_BUFFER
    {
        // Header - 8 bytes
        public uint ReparseTag;
        public ushort ReparseDataLength;
        public ushort Reserved;

        // Reparse point data
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4000)] // 16 KB
        public byte[] DataBuffer;

        //
        // The array causes issues here because a non-object type (e.g., ushort, uint) can't overlap an object (the byte[]).
        //

        //// REPARSE_DATA_BUFFER_SYMBOLICLINK
        //// 16 bytes
        //[FieldOffset(8)]
        //public ushort SymLinkSubstituteNameOffset;

        //[FieldOffset(10)]
        //public ushort SymLinkSubstituteNameLength;

        //[FieldOffset(12)]
        //public ushort SymLinkPrintNameOffset;

        //[FieldOffset(14)]
        //public ushort SymLinkPrintNameLength;

        //[FieldOffset(16)]
        //public uint SymLinkFlags;

        //[FieldOffset(20)]
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3FF0)] // 16 Kb - 16 bytes
        //public byte[] SymLinkPathBuffer;

        //// REPARSE_DATA_BUFFER_MOUNTPOINT
        //// 12 bytes
        //[FieldOffset(8)]
        //public ushort MntPtSubstituteNameOffset;

        //[FieldOffset(10)]
        //public ushort MntPtSubstituteNameLength;

        //[FieldOffset(12)]
        //public ushort MntPtPrintNameOffset;

        //[FieldOffset(14)]
        //public ushort MntPtPrintNameLength;

        //[FieldOffset(18)]
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3FF4)] // 16 Kb - 12 bytes
        //public byte[] MntPtPathBuffer;
    }
}