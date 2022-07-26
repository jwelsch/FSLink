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

        bool DeviceIoControl(IntPtr hDevice,
                             uint dwIoControlCode,
                             IntPtr InBuffer,
                             int nInBufferSize);

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

        bool OpenProcessToken(IntPtr hProcessHandle,
                              uint dwDesiredAccess,
                              out IntPtr hTokenHandle);

        bool LookupPrivilegeValue(string lpSystemName,
                                  string lpName,
                                  ref LUID lpLuid);

        bool AdjustTokenPrivileges(IntPtr hTokenHandle,
                                   [MarshalAs(UnmanagedType.Bool)] bool fDisableAllPrivileges,
                                   ref TOKEN_PRIVILEGES lpNewState,
                                   uint dwBufferLengthInBytes,
                                   ref TOKEN_PRIVILEGES lpPreviousState,
                                   out uint dwReturnLengthInBytes);

        bool AdjustTokenPrivileges(IntPtr TokenHandle,
                                   [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges,
                                   ref TOKEN_PRIVILEGES NewState);

        bool CloseHandle(IntPtr hObject);
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

            [DllImport(PinvokeDllNames.DeviceIoControlDllName, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
            public static extern bool DeviceIoControl(IntPtr hDevice,
                                                      uint dwIoControlCode,
                                                      IntPtr InBuffer,
                                                      int nInBufferSize,
                                                      IntPtr OutBuffer = default,
                                                      int nOutBufferSize = 0,
                                                      IntPtr pBytesReturned = default,
                                                      IntPtr lpOverlapped = default);

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

            [DllImport("advapi32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool OpenProcessToken(IntPtr hProcessHandle,
                                                       uint dwDesiredAccess,
                                                       out IntPtr hTokenHandle);

            [DllImport("advapi32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool LookupPrivilegeValue(string lpSystemName,
                                                           string lpName,
                                                           ref LUID lpLuid);

            [DllImport("advapi32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AdjustTokenPrivileges(IntPtr hTokenHandle,
                                                            [MarshalAs(UnmanagedType.Bool)] bool fDisableAllPrivileges,
                                                            ref TOKEN_PRIVILEGES lpNewState,
                                                            uint dwBufferLengthInBytes,
                                                            ref TOKEN_PRIVILEGES lpPreviousState,
                                                            out uint dwReturnLengthInBytes);

            [DllImport("advapi32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AdjustTokenPrivileges(IntPtr hTokenHandle,
                                                            [MarshalAs(UnmanagedType.Bool)] bool fDisableAllPrivileges,
                                                            ref TOKEN_PRIVILEGES lpNewState,
                                                            uint dwBufferLengthInBytes = 0,
                                                            IntPtr lpPreviousState = default,
                                                            IntPtr dwReturnLengthInBytes = default);

            [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CloseHandle(IntPtr hObject);
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

        public bool DeviceIoControl(IntPtr hDevice,
                     uint dwIoControlCode,
                     IntPtr InBuffer,
                     int nInBufferSize)
        {
            return NativeMethods.DeviceIoControl(hDevice, dwIoControlCode, InBuffer, nInBufferSize);
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

        public bool OpenProcessToken(IntPtr hProcessHandle,
                                     uint dwDesiredAccess,
                                     out IntPtr hTokenHandle)
        {
            return NativeMethods.OpenProcessToken(hProcessHandle, dwDesiredAccess, out hTokenHandle);
        }

        public bool LookupPrivilegeValue(string lpSystemName,
                                         string lpName,
                                         ref LUID lpLuid)
        {
            return NativeMethods.LookupPrivilegeValue(lpSystemName, lpName, ref lpLuid);
        }

        public bool AdjustTokenPrivileges(IntPtr hTokenHandle,
                                          [MarshalAs(UnmanagedType.Bool)] bool fDisableAllPrivileges,
                                          ref TOKEN_PRIVILEGES lpNewState,
                                          uint dwBufferLengthInBytes,
                                          ref TOKEN_PRIVILEGES lpPreviousState,
                                          out uint dwReturnLengthInBytes)
        {
            return NativeMethods.AdjustTokenPrivileges(hTokenHandle, fDisableAllPrivileges, ref lpNewState, dwBufferLengthInBytes, ref lpPreviousState, out dwReturnLengthInBytes);
        }

        public bool AdjustTokenPrivileges(IntPtr hTokenHandle,
                                          [MarshalAs(UnmanagedType.Bool)] bool fDisableAllPrivileges,
                                          ref TOKEN_PRIVILEGES lpNewState)
        {
            return NativeMethods.AdjustTokenPrivileges(hTokenHandle, fDisableAllPrivileges, ref lpNewState);
        }

        public bool CloseHandle(IntPtr hObject)
        {
            return NativeMethods.CloseHandle(hObject);
        }
    }
}
