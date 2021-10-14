using System;
using System.Runtime.InteropServices;

namespace FSLinkLib.PInvoke
{
    internal interface INativeMethodCaller
    {
        bool DeviceIoControl(IntPtr hDevice,
                             uint dwIoControlCode,
                             IntPtr InBuffer,
                             int nInBufferSize,
                             IntPtr OutBuffer,
                             int nOutBufferSize,
                             out int pBytesReturned,
                             IntPtr lpOverlapped);

        bool GetFileInformationByHandle(IntPtr hFile,
                                        out BY_HANDLE_FILE_INFORMATION lpFileInformation);

        IntPtr CreateFile(string lpFileName,
                          FileDesiredAccess dwDesiredAccess,
                          FileShareMode dwShareMode,
                          IntPtr lpSecurityAttributes,
                          FileCreationDisposition dwCreationDisposition,
                          FileAttributes dwFlagsAndAttributes,
                          IntPtr hTemplateFile);

        IntPtr FindFirstFileEx(string lpFileName,
                               FINDEX_INFO_LEVELS fInfoLevelId,
                               out WIN32_FIND_DATA lpFindFileData,
                               FINDEX_SEARCH_OPS fSearchOp,
                               IntPtr lpSearchFilter,
                               int dwAdditionalFlags);

        bool FindClose(IntPtr handle);

        bool CreateSymbolicLink(string lpSymlinkFileName,
                                string lpTargetFileName,
                                SYMBOLIC_LINK_FLAG dwFlags);

        bool CreateHardLink(string lpFileName,
                            string lpExistingFileName,
                            IntPtr lpSecurityAttributes);
    }

    internal class NativeMethodCaller : INativeMethodCaller
    {
        private static class NativeMethods
        {
            [DllImport(PinvokeDllNames.DeviceIoControlDllName, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
            public static extern bool DeviceIoControl(IntPtr hDevice,
                                                      uint dwIoControlCode,
                                                      IntPtr InBuffer,
                                                      int nInBufferSize,
                                                      IntPtr OutBuffer,
                                                      int nOutBufferSize,
                                                      out int pBytesReturned,
                                                      IntPtr lpOverlapped);

            [DllImport(PinvokeDllNames.GetFileInformationByHandleDllName, SetLastError = true, CharSet = CharSet.Unicode)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetFileInformationByHandle(IntPtr hFile,
                                                                 out BY_HANDLE_FILE_INFORMATION lpFileInformation);

            [DllImport(PinvokeDllNames.CreateFileDllName, SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr CreateFile(string lpFileName,
                                                   FileDesiredAccess dwDesiredAccess,
                                                   FileShareMode dwShareMode,
                                                   IntPtr lpSecurityAttributes,
                                                   FileCreationDisposition dwCreationDisposition,
                                                   FileAttributes dwFlagsAndAttributes,
                                                   IntPtr hTemplateFile);

            [DllImport(PinvokeDllNames.FindFirstFileDllName, /*EntryPoint = "FindFirstFileExW",*/ SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr FindFirstFileEx(string lpFileName,
                                                        FINDEX_INFO_LEVELS fInfoLevelId,
                                                        out WIN32_FIND_DATA lpFindFileData,
                                                        FINDEX_SEARCH_OPS fSearchOp,
                                                        IntPtr lpSearchFilter,
                                                        int dwAdditionalFlags);

            [DllImport(PinvokeDllNames.FindCloseDllName)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool FindClose(IntPtr handle);

            [DllImport(PinvokeDllNames.CreateSymbolicLinkDllName, SetLastError = true, CharSet = CharSet.Unicode)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool CreateSymbolicLink(string lpSymlinkFileName,
                                                         string lpTargetFileName,
                                                         SYMBOLIC_LINK_FLAG dwFlags);

            [DllImport(PinvokeDllNames.CreateHardLinkDllName, SetLastError = true, CharSet = CharSet.Auto)]
            public static extern bool CreateHardLink(string lpFileName,
                                                     string lpExistingFileName,
                                                     IntPtr lpSecurityAttributes);

            [DllImport(PinvokeDllNames.DeleteFileDllName, SetLastError = true, CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DeleteFile(string lpFileName);
        }

        public bool DeviceIoControl(IntPtr hDevice,
                                    uint dwIoControlCode,
                                    IntPtr InBuffer,
                                    int nInBufferSize,
                                    IntPtr OutBuffer,
                                    int nOutBufferSize,
                                    out int pBytesReturned,
                                    IntPtr lpOverlapped)
        {
            return NativeMethods.DeviceIoControl(hDevice, dwIoControlCode, InBuffer, nInBufferSize, OutBuffer, nOutBufferSize, out pBytesReturned, lpOverlapped);
        }

        public bool GetFileInformationByHandle(IntPtr hFile,
                                               out BY_HANDLE_FILE_INFORMATION lpFileInformation)
        {
            return NativeMethods.GetFileInformationByHandle(hFile, out lpFileInformation);
        }

        public IntPtr CreateFile(string lpFileName,
                                 FileDesiredAccess dwDesiredAccess,
                                 FileShareMode dwShareMode,
                                 IntPtr lpSecurityAttributes,
                                 FileCreationDisposition dwCreationDisposition,
                                 FileAttributes dwFlagsAndAttributes,
                                 IntPtr hTemplateFile)
        {
            return NativeMethods.CreateFile(lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
        }

        public IntPtr FindFirstFileEx(string lpFileName,
                                      FINDEX_INFO_LEVELS fInfoLevelId,
                                      out WIN32_FIND_DATA lpFindFileData,
                                      FINDEX_SEARCH_OPS fSearchOp,
                                      IntPtr lpSearchFilter,
                                      int dwAdditionalFlags)
        {
            return NativeMethods.FindFirstFileEx(lpFileName, fInfoLevelId, out lpFindFileData, fSearchOp, lpSearchFilter, dwAdditionalFlags);
        }

        public bool FindClose(IntPtr handle)
        {
            return NativeMethods.FindClose(handle);
        }

        public bool CreateSymbolicLink(string lpSymlinkFileName,
                                       string lpTargetFileName,
                                       SYMBOLIC_LINK_FLAG dwFlags)
        {
            return NativeMethods.CreateSymbolicLink(lpSymlinkFileName, lpTargetFileName, dwFlags);
        }

        public bool CreateHardLink(string lpFileName,
                                   string lpExistingFileName,
                                   IntPtr lpSecurityAttributes)
        {
            return NativeMethods.CreateHardLink(lpFileName, lpExistingFileName, lpSecurityAttributes);
        }
    }
}
