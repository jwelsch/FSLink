using FSLinkLib.PInvoke;
using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

#nullable enable

namespace FSLinkLib
{
    //
    // Based on:
    // https://github.com/PowerShell/PowerShell/blob/cfe45defc1ed87a7a7b916f154e56124d81a320b/src/System.Management.Automation/namespaces/FileSystemProvider.cs#L7886
    //

    public enum SymbolicLinkType
    {
        File,
        Directory
    }

    public interface IFileSystemLink
    {
        FileSystemLinkType GetLinkType(string path);

        string? GetLinkTarget(string path);

        bool IsHardLink(string filePath);

        bool IsJunction(string directoryPath);

        bool IsSymbolicLink(string path);

        bool CreateHardLink(string hardLinkPath, string targetFilePath);

        bool CreateJunction(string junctionPath, string targetDirectoryPath);

        bool CreateSymbolicLink(string symbolicLinkPath, string targetPath, SymbolicLinkType linkType);

        void DeleteHardLink(string hardLinkPath);

        void DeleteJunction(string junctionPath);

        void DeleteSymbolicLink(string symbolicLinkPath);

        IReparsePoint? GetReparsePoint(string path);
    }

    /// <summary>
    /// Class to find the symbolic link target.
    /// </summary>
    internal class FileSystemLink : IFileSystemLink
    {
        private readonly INativeMethodCaller _nativeMethodCaller;

        public FileSystemLink(INativeMethodCaller nativeMethodCaller)
        {
            _nativeMethodCaller = nativeMethodCaller;
        }

        /// <summary>
        /// Gets the target of the specified reparse point.
        /// </summary>
        /// <param name="instance">The object of FileInfo or DirectoryInfo type.</param>
        /// <returns>The target of the reparse point.</returns>
        public string? GetTarget(FileSystemInfo fileInfo)
        {
            return WinInternalGetTarget(fileInfo.FullName);
        }

        public FileSystemLinkType GetLinkType(FileSystemInfo fileInfo)
        {
            return WinInternalGetLinkType(fileInfo.FullName);
        }

        private FileSystemLinkType WinInternalGetLinkType(string filePath)
        {
            // We set accessMode parameter to zero because documentation says:
            // If this parameter is zero, the application can query certain metadata
            // such as file, directory, or device attributes without accessing
            // that file or device, even if GENERIC_READ access would have been denied.
            using (var handle = OpenReparsePoint(filePath, FileDesiredAccess.GenericZero))
            {
                var outBufferSize = Marshal.SizeOf<REPARSE_DATA_BUFFER_SYMBOLICLINK>();

                var outBuffer = Marshal.AllocHGlobal(outBufferSize);
                var success = false;

                try
                {
                    var bytesReturned = 0;

                    // OACR warning 62001 about using DeviceIOControl has been disabled.
                    // According to MSDN guidance DangerousAddRef() and DangerousRelease() have been used.
                    handle.DangerousAddRef(ref success);

                    // Get Buffer size
                    var dangerousHandle = handle.DangerousGetHandle();

                    var result = _nativeMethodCaller.DeviceIoControl(dangerousHandle,
                                                                     Constants.FSCTL_GET_REPARSE_POINT,
                                                                     IntPtr.Zero,
                                                                     0,
                                                                     outBuffer,
                                                                     outBufferSize,
                                                                     out bytesReturned,
                                                                     IntPtr.Zero);

                    if (!result)
                    {
                        // It's not a reparse point or the file system doesn't support reparse points.
                        return IsHardLink(ref dangerousHandle) ? FileSystemLinkType.HardLink : FileSystemLinkType.None;
                    }

                    var reparseDataBuffer = Marshal.PtrToStructure<REPARSE_DATA_BUFFER_SYMBOLICLINK>(outBuffer);

                    return reparseDataBuffer.ReparseTag switch
                    {
                        Constants.IO_REPARSE_TAG_SYMLINK => FileSystemLinkType.SymbolicLink,
                        Constants.IO_REPARSE_TAG_MOUNT_POINT => FileSystemLinkType.Junction,
                        _ => FileSystemLinkType.None,
                    };
                }
                finally
                {
                    if (success)
                    {
                        handle.DangerousRelease();
                    }

                    Marshal.FreeHGlobal(outBuffer);
                }
            }
        }

        private bool WinIsHardLink(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file at path '{filePath}' does not exist.", filePath);
            }

            var attributes = File.GetAttributes(filePath);

            // only check for hard link if the item is not directory
            if ((attributes & System.IO.FileAttributes.Directory) != System.IO.FileAttributes.Directory)
            {
                return false;
            }

            var nativeHandle = _nativeMethodCaller.CreateFile(filePath,
                                                              FileDesiredAccess.GenericRead,
                                                              FileShareMode.Read,
                                                              IntPtr.Zero,
                                                              FileCreationDisposition.OpenExisting,
                                                              PInvoke.FileAttributes.Normal,
                                                              IntPtr.Zero);

            using var handle = new SafeFileHandle(nativeHandle, true);
            var success = false;

            try
            {
                handle.DangerousAddRef(ref success);
                var dangerousHandle = handle.DangerousGetHandle();

                return IsHardLink(ref dangerousHandle);
            }
            finally
            {
                if (success)
                {
                    handle.DangerousRelease();
                }
            }
        }

        private bool WinIsHardLink(ref IntPtr handle)
        {
            var succeeded = _nativeMethodCaller.GetFileInformationByHandle(handle, out BY_HANDLE_FILE_INFORMATION handleInfo);
            return succeeded && (handleInfo.NumberOfLinks > 1);
        }

        private bool IsHardLink(ref IntPtr handle)
        {
            return WinIsHardLink(ref handle);
        }

        private string? WinInternalGetTarget(string path)
        {
            // We set accessMode parameter to zero because documentation says:
            // If this parameter is zero, the application can query certain metadata
            // such as file, directory, or device attributes without accessing
            // that file or device, even if GENERIC_READ access would have been denied.
            using var handle = OpenReparsePoint(path, FileDesiredAccess.GenericZero);
            return WinInternalGetTarget(handle);
        }

        private string? WinInternalGetTarget(SafeFileHandle handle)
        {
            var outBufferSize = Marshal.SizeOf<REPARSE_DATA_BUFFER_SYMBOLICLINK>();

            var outBuffer = Marshal.AllocHGlobal(outBufferSize);
            var success = false;

            try
            {
                // OACR warning 62001 about using DeviceIOControl has been disabled.
                // According to MSDN guidance DangerousAddRef() and DangerousRelease() have been used.
                handle.DangerousAddRef(ref success);

                var result = _nativeMethodCaller.DeviceIoControl(handle.DangerousGetHandle(),
                                                                 Constants.FSCTL_GET_REPARSE_POINT,
                                                                 IntPtr.Zero,
                                                                 0,
                                                                 outBuffer,
                                                                 outBufferSize,
                                                                 out int bytesReturned,
                                                                 IntPtr.Zero);

                if (!result)
                {
                    // It's not a reparse point or the file system doesn't support reparse points.
                    return null;
                }

                string? targetDir = null;

                var reparseDataBuffer = Marshal.PtrToStructure<REPARSE_DATA_BUFFER_SYMBOLICLINK>(outBuffer);

                switch (reparseDataBuffer.ReparseTag)
                {
                    case Constants.IO_REPARSE_TAG_SYMLINK:
                        targetDir = Encoding.Unicode.GetString(reparseDataBuffer.PathBuffer, reparseDataBuffer.SubstituteNameOffset, reparseDataBuffer.SubstituteNameLength);
                        break;

                    case Constants.IO_REPARSE_TAG_MOUNT_POINT:
                        var reparseMountPointDataBuffer = Marshal.PtrToStructure<REPARSE_DATA_BUFFER_MOUNTPOINT>(outBuffer);
                        targetDir = Encoding.Unicode.GetString(reparseMountPointDataBuffer.PathBuffer, reparseMountPointDataBuffer.SubstituteNameOffset, reparseMountPointDataBuffer.SubstituteNameLength);
                        break;

                    default:
                        return null;
                }

                if (targetDir != null && targetDir.StartsWith(Constants.NonInterpretedPathPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    targetDir = targetDir.Substring(Constants.NonInterpretedPathPrefix.Length);
                }

                return targetDir;
            }
            finally
            {
                if (success)
                {
                    handle.DangerousRelease();
                }

                Marshal.FreeHGlobal(outBuffer);
            }
        }

        private bool WinCreateJunction(string path, string target)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (!string.IsNullOrEmpty(target))
                {
                    using (var handle = OpenReparsePoint(path, FileDesiredAccess.GenericWrite))
                    {
                        var mountPointBytes = Encoding.Unicode.GetBytes(Constants.NonInterpretedPathPrefix + Path.GetFullPath(target));

                        var mountPoint = new REPARSE_DATA_BUFFER_MOUNTPOINT();
                        mountPoint.ReparseTag = Constants.IO_REPARSE_TAG_MOUNT_POINT;
                        mountPoint.ReparseDataLength = (ushort)(mountPointBytes.Length + 12); // Added space for the header and null endo
                        mountPoint.SubstituteNameOffset = 0;
                        mountPoint.SubstituteNameLength = (ushort)mountPointBytes.Length;
                        mountPoint.PrintNameOffset = (ushort)(mountPointBytes.Length + 2); // 2 as unicode null take 2 bytes.
                        mountPoint.PrintNameLength = 0;
                        mountPoint.PathBuffer = new byte[0x3FF0]; // Buffer for max size.
                        Array.Copy(mountPointBytes, mountPoint.PathBuffer, mountPointBytes.Length);

                        var nativeBufferSize = Marshal.SizeOf(mountPoint);
                        var nativeBuffer = Marshal.AllocHGlobal(nativeBufferSize);
                        var success = false;

                        try
                        {
                            Marshal.StructureToPtr(mountPoint, nativeBuffer, false);

                            // OACR warning 62001 about using DeviceIOControl has been disabled.
                            // According to MSDN guidance DangerousAddRef() and DangerousRelease() have been used.
                            handle.DangerousAddRef(ref success);

                            var result = _nativeMethodCaller.DeviceIoControl(handle.DangerousGetHandle(), Constants.FSCTL_SET_REPARSE_POINT, nativeBuffer, mountPointBytes.Length + 20, IntPtr.Zero, 0, out int bytesReturned, IntPtr.Zero);

                            if (!result)
                            {
                                throw new Win32Exception(Marshal.GetLastWin32Error());
                            }

                            return result;
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(nativeBuffer);

                            if (success)
                            {
                                handle.DangerousRelease();
                            }
                        }
                    }
                }
                else
                {
                    throw new ArgumentNullException(nameof(target));
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(path));
            }
        }

        private SafeFileHandle OpenReparsePoint(string reparsePoint, FileDesiredAccess accessMode)
        {
            return WinOpenReparsePoint(reparsePoint, accessMode);
        }

        private SafeFileHandle WinOpenReparsePoint(string reparsePoint, FileDesiredAccess accessMode)
        {
            var nativeHandle = _nativeMethodCaller.CreateFile(reparsePoint,
                                                              accessMode,
                                                              FileShareMode.Read | FileShareMode.Write | FileShareMode.Delete,
                                                              IntPtr.Zero,
                                                              FileCreationDisposition.OpenExisting,
                                                              PInvoke.FileAttributes.BackupSemantics | PInvoke.FileAttributes.OpenReparsePoint,
                                                              IntPtr.Zero);

            var lastError = Marshal.GetLastWin32Error();

            if (lastError != 0)
            {
                throw new Win32Exception(lastError);
            }

            return new SafeFileHandle(nativeHandle, true);
        }

        public FileSystemLinkType GetLinkType(string path)
        {
            return WinInternalGetLinkType(path);
        }

        public string? GetLinkTarget(string path)
        {
            return WinInternalGetTarget(path);
        }

        public bool IsHardLink(string filePath)
        {
            return GetLinkType(filePath) == FileSystemLinkType.HardLink;
        }

        public bool IsJunction(string directoryPath)
        {
            return GetLinkType(directoryPath) == FileSystemLinkType.Junction;
        }

        public bool IsSymbolicLink(string path)
        {
            return GetLinkType(path) == FileSystemLinkType.SymbolicLink;
        }

        public bool CreateHardLink(string hardLinkPath, string targetFilePath)
        {
            return _nativeMethodCaller.CreateHardLink(hardLinkPath, targetFilePath, IntPtr.Zero);
        }

        public bool CreateJunction(string junctionPath, string targetDirectoryPath)
        {
            return WinCreateJunction(junctionPath, targetDirectoryPath);
        }

        public bool CreateSymbolicLink(string symbolicLinkPath, string targetPath, SymbolicLinkType linkType)
        {
            var flag = linkType switch
            {
                SymbolicLinkType.File => SYMBOLIC_LINK_FLAG.File,
                SymbolicLinkType.Directory => SYMBOLIC_LINK_FLAG.Directory,
                _ => throw new ArgumentException($"Unknown {nameof(SymbolicLinkType)} value '{linkType}'.", nameof(linkType))
            };

            return _nativeMethodCaller.CreateSymbolicLink(symbolicLinkPath, targetPath, flag);
        }

        public void DeleteHardLink(string hardLinkPath)
        {
            File.Delete(hardLinkPath);
        }

        public void DeleteJunction(string junctionPath)
        {
            Directory.Delete(junctionPath);
        }

        public void DeleteSymbolicLink(string symbolicLinkPath)
        {
            if ((File.GetAttributes(symbolicLinkPath) & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory)
            {
                Directory.Delete(symbolicLinkPath);
            }
            else
            {
                File.Delete(symbolicLinkPath);
            }
        }

        public IReparsePoint? GetReparsePoint(string path)
        {
            WIN32_FIND_DATA findData;
            IntPtr findHandle = default;

            try
            {
                findHandle = _nativeMethodCaller.FindFirstFileEx(path, FINDEX_INFO_LEVELS.FindExInfoBasic, out findData, FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, 0);

                if (findHandle.ToInt64() != -1 && findHandle != IntPtr.Zero)
                {
                    if ((findData.dwFileAttributes & (uint)System.IO.FileAttributes.ReparsePoint) == (uint)System.IO.FileAttributes.ReparsePoint)
                    {
                        return new ReparsePoint(path, (System.IO.FileAttributes)findData.dwFileAttributes, new ReparseTag(findData.dwReserved0));
                    }
                }

                var lastError = Marshal.GetLastWin32Error();

                if (lastError != 0)
                {
                    throw new Win32Exception(lastError, $"Error getting reparse point data for path '{path}'");
                }
            }
            finally
            {
                if (findHandle.ToInt64() != -1 && findHandle != IntPtr.Zero)
                {
                    _nativeMethodCaller.FindClose(findHandle);
                }
            }

            return null;
        }

        //public static bool IsReparsePoint(FileSystemInfo fileInfo)
        //{
        //    return fileInfo.Attributes.HasFlag(System.IO.FileAttributes.ReparsePoint);
        //}

        //public static bool IsReparsePointLikeSymlink(FileSystemInfo fileInfo)
        //{
        //    WIN32_FIND_DATA data = default;
        //    var fullPath = Path.TrimEndingDirectorySeparator(fileInfo.FullName);
        //    if (fullPath.Length > Constants.MAX_PATH)
        //    {
        //        fullPath = PathUtils.EnsureExtendedPrefix(fullPath);
        //    }

        //    using (var handle = FindFirstFileEx(fullPath, FINDEX_INFO_LEVELS.FindExInfoBasic, ref data, FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, 0))
        //    {
        //        if (handle.IsInvalid)
        //        {
        //            // Our handle could be invalidated by something else touching the filesystem,
        //            // so ensure we deal with that possibility here
        //            var lastError = Marshal.GetLastWin32Error();
        //            throw new Win32Exception(lastError);
        //        }

        //        // We already have the file attribute information from our Win32 call,
        //        // so no need to take the expense of the FileInfo.FileAttributes call
        //        const int FILE_ATTRIBUTE_REPARSE_POINT = 0x0400;
        //        if ((data.dwFileAttributes & FILE_ATTRIBUTE_REPARSE_POINT) == 0)
        //        {
        //            // Not a reparse point.
        //            return false;
        //        }

        //        // The name surrogate bit 0x20000000 is defined in https://docs.microsoft.com/windows/win32/fileio/reparse-point-tags
        //        // Name surrogates (0x20000000) are reparse points that point to other named entities local to the filesystem
        //        // (like symlinks and mount points).
        //        // In the case of OneDrive, they are not name surrogates and would be safe to recurse into.
        //        if ((data.dwReserved0 & 0x20000000) == 0 && (data.dwReserved0 != Constants.IO_REPARSE_TAG_APPEXECLINK))
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        //public static bool IsSameFileSystemItem(string pathOne, string pathTwo)
        //{
        //    return WinIsSameFileSystemItem(pathOne, pathTwo);
        //}

        //private static bool WinIsSameFileSystemItem(string pathOne, string pathTwo)
        //{
        //    const FileAccess access = FileAccess.Read;
        //    const FileShare share = FileShare.Read;
        //    const FileMode creation = FileMode.Open;
        //    const PInvoke.FileAttributes attributes = PInvoke.FileAttributes.BackupSemantics | PInvoke.FileAttributes.PosixSemantics;

        //    using (var sfOne = AlternateDataStreamUtilities.NativeMethods.CreateFile(pathOne, access, share, IntPtr.Zero, creation, (int)attributes, IntPtr.Zero))
        //    using (var sfTwo = AlternateDataStreamUtilities.NativeMethods.CreateFile(pathTwo, access, share, IntPtr.Zero, creation, (int)attributes, IntPtr.Zero))
        //    {
        //        if (!sfOne.IsInvalid && !sfTwo.IsInvalid)
        //        {
        //            BY_HANDLE_FILE_INFORMATION infoOne;
        //            BY_HANDLE_FILE_INFORMATION infoTwo;
        //            if (GetFileInformationByHandle(sfOne.DangerousGetHandle(), out infoOne)
        //                && GetFileInformationByHandle(sfTwo.DangerousGetHandle(), out infoTwo))
        //            {
        //                return infoOne.VolumeSerialNumber == infoTwo.VolumeSerialNumber
        //                       && infoOne.FileIndexHigh == infoTwo.FileIndexHigh
        //                       && infoOne.FileIndexLow == infoTwo.FileIndexLow;
        //            }
        //        }
        //    }

        //    return false;
        //}

        //public static bool GetInodeData(string path, out System.ValueTuple<UInt64, UInt64> inodeData)
        //{
        //    return WinGetInodeData(path, out inodeData);
        //}

        //private static bool WinGetInodeData(string path, out System.ValueTuple<UInt64, UInt64> inodeData)
        //{
        //    const FileAccess access = FileAccess.Read;
        //    const FileShare share = FileShare.Read;
        //    const FileMode creation = FileMode.Open;
        //    const PInvoke.FileAttributes attributes = PInvoke.FileAttributes.BackupSemantics | PInvoke.FileAttributes.PosixSemantics;

        //    using (var sf = AlternateDataStreamUtilities.NativeMethods.CreateFile(path, access, share, IntPtr.Zero, creation, (int)attributes, IntPtr.Zero))
        //    {
        //        if (!sf.IsInvalid)
        //        {
        //            BY_HANDLE_FILE_INFORMATION info;

        //            if (GetFileInformationByHandle(sf.DangerousGetHandle(), out info))
        //            {
        //                var tmp = info.FileIndexHigh;
        //                tmp = (tmp << 32) | info.FileIndexLow;

        //                inodeData = (info.VolumeSerialNumber, tmp);

        //                return true;
        //            }
        //        }
        //    }

        //    inodeData = (0, 0);

        //    return false;
        //}
    }
}
