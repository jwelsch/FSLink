using FSLinkLib.PInvoke;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FSLinkLib
{
    public interface IFileSystemPrivilege
    {
        void EnsureRestoreName();
    }

    internal class FileSystemPrivilege : IFileSystemPrivilege
    {
        private readonly INativeMethodCaller _nativeMethodCaller;

        public FileSystemPrivilege(INativeMethodCaller nativeMethodCaller)
        {
            _nativeMethodCaller = nativeMethodCaller;
        }

        public void EnsureRestoreName()
        {
            EnsureSecurityPrivilege(Constants.SE_RESTORE_NAME);
        }

        protected void EnsureSecurityPrivilege(string privilege)
        {
            //HANDLE hToken;
            //TOKEN_PRIVILEGES tp;
            //OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES, &hToken);
            //LookupPrivilegeValue(NULL, SE_RESTORE_NAME, &tp.Privileges[0].Luid);

            //tp.PrivilegeCount = 1;
            //tp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

            //AdjustTokenPrivileges(hToken, FALSE, &tp, sizeof(TOKEN_PRIVILEGES), NULL, NULL);
            //CloseHandle(hToken);
            
            var process = Process.GetCurrentProcess();
            var hToken = IntPtr.Zero;

            try
            {
                if (!_nativeMethodCaller.OpenProcessToken(process.Handle, Constants.TOKEN_ADJUST_PRIVILEGES, out hToken))
                {
                    var error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(error, $"Failed to open process token.");
                }

                var tokenPrivileges = new TOKEN_PRIVILEGES()
                {
                    Privileges = new LUID_AND_ATTRIBUTES[1]
                };

                if (!_nativeMethodCaller.LookupPrivilegeValue(null, privilege, ref tokenPrivileges.Privileges[0].Luid))
                {
                    var error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(error, $"Failed to look up privilege value.");
                }

                tokenPrivileges.PrivilegeCount += 1;
                tokenPrivileges.Privileges[0].Attributes |= Constants.SE_PRIVILEGE_ENABLED;

                if (!_nativeMethodCaller.AdjustTokenPrivileges(hToken, false, ref tokenPrivileges))
                {
                    var error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(error, $"Failed to adjust token privileges.");
                }
            }
            finally
            {
                if (hToken != IntPtr.Zero)
                {
                    if (!_nativeMethodCaller.CloseHandle(hToken))
                    {
                        var error = Marshal.GetLastWin32Error();
                        throw new Win32Exception(error, $"Failed to close handle.");
                    }
                }
            }
        }
    }
}
