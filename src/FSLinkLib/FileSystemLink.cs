using FSLinkCommon.Wraps;
using FSLinkLib.PInvoke;
using FSLinkLib.ReparsePoints;
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

        void CreateHardLink(string hardLinkPath, string targetFilePath);

        void CreateJunction(string junctionPath, string targetDirectoryPath);

        void CreateSymbolicLink(string symbolicLinkPath, string targetPath, SymbolicLinkType linkType);

        void DeleteHardLink(string hardLinkPath);

        void DeleteJunction(string junctionPath);

        void DeleteSymbolicLink(string symbolicLinkPath);

        IReparsePoint? GetReparsePoint(string path);

        void DeleteReparsePoint(string path);
    }

    /// <summary>
    /// Class to find the symbolic link target.
    /// </summary>
    internal class FileSystemLink : IFileSystemLink
    {
        private readonly INativeMethodCaller _nativeMethodCaller;
        private readonly IFileWrap _fileWrap;

        public FileSystemLink(INativeMethodCaller nativeMethodCaller, IFileWrap fileWrap)
        {
            _nativeMethodCaller = nativeMethodCaller;
            _fileWrap = fileWrap;
        }

        public FileSystemLinkType GetLinkType(string path)
        {
            // We set accessMode parameter to zero because documentation says:
            // If this parameter is zero, the application can query certain metadata
            // such as file, directory, or device attributes without accessing
            // that file or device, even if GENERIC_READ access would have been denied.
            using var handle = OpenReparsePoint(path, FileDesiredAccess.GenericZero);

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

        public string? GetLinkTarget(string path)
        {
            // We set accessMode parameter to zero because documentation says:
            // If this parameter is zero, the application can query certain metadata
            // such as file, directory, or device attributes without accessing
            // that file or device, even if GENERIC_READ access would have been denied.
            using var handle = OpenReparsePoint(path, FileDesiredAccess.GenericZero);

            return WinInternalGetTarget(handle);
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

        public void CreateHardLink(string hardLinkPath, string targetFilePath)
        {
            var result = _nativeMethodCaller.CreateHardLink(hardLinkPath, targetFilePath, IntPtr.Zero);

            if (!result)
            {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error, $"Error creating hard link for path '{hardLinkPath}'");
            }
        }

        public void CreateJunction(string junctionPath, string targetDirectoryPath)
        {
            if (string.IsNullOrEmpty(junctionPath))
            {
                throw new ArgumentNullException(nameof(junctionPath));
            }

            if (string.IsNullOrEmpty(targetDirectoryPath))
            {
                throw new ArgumentNullException(nameof(targetDirectoryPath));
            }

            using var handle = OpenReparsePoint(junctionPath, FileDesiredAccess.GenericWrite);

            var mountPointBytes = Encoding.Unicode.GetBytes(Constants.NonInterpretedPathPrefix + Path.GetFullPath(targetDirectoryPath));

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
                    throw new Win32Exception(Marshal.GetLastWin32Error(), $"Error setting reparse point for path '{junctionPath}'");
                }
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

        public void CreateSymbolicLink(string symbolicLinkPath, string targetPath, SymbolicLinkType linkType)
        {
            var flag = linkType switch
            {
                SymbolicLinkType.File => SYMBOLIC_LINK_FLAG.File,
                SymbolicLinkType.Directory => SYMBOLIC_LINK_FLAG.Directory,
                _ => throw new ArgumentException($"Unknown {nameof(SymbolicLinkType)} value '{linkType}'.", nameof(linkType))
            };

            var result = _nativeMethodCaller.CreateSymbolicLink(symbolicLinkPath, targetPath, flag);

            if (!result)
            {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error, $"Error creating symbolic link at path '{symbolicLinkPath}' with target '{targetPath}'");
            }
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

        //public IReparsePoint? GetReparsePoint(string path)
        //{
        //    WIN32_FIND_DATA findData;
        //    IntPtr findHandle = default;

        //    try
        //    {
        //        findHandle = _nativeMethodCaller.FindFirstFileEx(path, FINDEX_INFO_LEVELS.FindExInfoBasic, out findData, FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, 0);

        //        if (findHandle.ToInt64() != -1 && findHandle != IntPtr.Zero)
        //        {
        //            if ((findData.dwFileAttributes & (uint)System.IO.FileAttributes.ReparsePoint) == (uint)System.IO.FileAttributes.ReparsePoint)
        //            {
        //                return new ReparsePoint(path, (System.IO.FileAttributes)findData.dwFileAttributes, new ReparseTag(findData.dwReserved0), 0, 0);
        //            }
        //        }

        //        var lastError = Marshal.GetLastWin32Error();

        //        if (lastError != 0)
        //        {
        //            throw new Win32Exception(lastError, $"Error getting reparse point data for path '{path}'");
        //        }
        //    }
        //    finally
        //    {
        //        if (findHandle.ToInt64() != -1 && findHandle != IntPtr.Zero)
        //        {
        //            _nativeMethodCaller.FindClose(findHandle);
        //        }
        //    }

        //    return null;
        //}

        public IReparsePoint? GetReparsePoint(string path)
        {
            var handle = OpenReparsePoint(path, FileDesiredAccess.GenericZero);

            var outBufferSize = Marshal.SizeOf<REPARSE_DATA_BUFFER>();
            var outBuffer = Marshal.AllocHGlobal(outBufferSize);
            var success = false;

            try
            {
                handle.DangerousAddRef(ref success);

                var result = _nativeMethodCaller.DeviceIoControl(handle.DangerousGetHandle(),
                                                                 Constants.FSCTL_GET_REPARSE_POINT,
                                                                 IntPtr.Zero,
                                                                 0,
                                                                 outBuffer,
                                                                 outBufferSize,
                                                                 out int bytesReturned,
                                                                 IntPtr.Zero);

                var lastError = Marshal.GetLastWin32Error();

                if (lastError != 0)
                {
                    throw new Win32Exception(lastError, $"Error getting reparse point data for path '{path}'");
                }

                var dataBuffer = Marshal.PtrToStructure<REPARSE_DATA_BUFFER>(outBuffer);
                var tagValue = ReparsePointBase.LookUpTag(new ReparseTag(dataBuffer.ReparseTag));
                var attributes = _fileWrap.GetAttributes(path);

                if (tagValue == ReparseTagValues.IO_REPARSE_TAG_SYMLINK)
                {
                    return new SymbolicLinkReparsePoint(path,
                                                        attributes,
                                                        Marshal.PtrToStructure<REPARSE_DATA_BUFFER_SYMBOLICLINK>(outBuffer));
                }
                else if (tagValue == ReparseTagValues.IO_REPARSE_TAG_MOUNT_POINT)
                {
                    return new MountPointReparsePoint(path,
                                                      attributes,
                                                      Marshal.PtrToStructure<REPARSE_DATA_BUFFER_MOUNTPOINT>(outBuffer));
                }
                else
                {
                    return new DataReparsePoint(path, attributes, dataBuffer);
                }
            }
            finally
            {
                if (success)
                {
                    handle.DangerousRelease();
                }

                Marshal.FreeHGlobal(outBuffer);
            }

#pragma warning disable CS0162 // Unreachable code detected
            return null;
#pragma warning restore CS0162 // Unreachable code detected
        }

        public void DeleteReparsePoint(string path)
        {
            var fileHandle = OpenReparsePoint(path, FileDesiredAccess.GenericAll);

            var success = false;

            try
            {
                fileHandle.DangerousAddRef(ref success);

                var result = _nativeMethodCaller.DeviceIoControl(fileHandle.DangerousGetHandle(), Constants.FSCTL_DELETE_REPARSE_POINT, IntPtr.Zero, 0, IntPtr.Zero, 0, out int bytesReturned, IntPtr.Zero);

                if (!result)
                {
                    var error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(error, $"Error deleting reparse point for path '{path}'");
                }
            }
            finally
            {
                if (success)
                {
                    fileHandle.DangerousRelease();
                }
            }
        }

        private bool IsHardLink(ref IntPtr handle)
        {
            var succeeded = _nativeMethodCaller.GetFileInformationByHandle(handle, out BY_HANDLE_FILE_INFORMATION handleInfo);
            return succeeded && (handleInfo.NumberOfLinks > 1);
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

        private SafeFileHandle OpenReparsePoint(string reparsePoint, FileDesiredAccess accessMode)
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
                throw new Win32Exception(lastError, $"Error accessing reparse point for path '{reparsePoint}'");
            }

            return new SafeFileHandle(nativeHandle, true);
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
