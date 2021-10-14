using System;
using System.IO;
using System.Runtime.CompilerServices;

#nullable enable

namespace FSLinkLib
{
    //
    // Based on:
    // https://github.com/PowerShell/PowerShell/blob/e2c23fc5bd035251bed71413ad7c467c99dafd76/src/System.Management.Automation/utils/PathUtils.cs
    //

    public static class PathUtils
    {
        // Code here is copied from .NET's internal path helper implementation:
        // https://github.com/dotnet/runtime/blob/dcce0f56e10f5ac9539354b049341a2d7c0cdebf/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.Windows.cs
        // It has been left as a verbatim copy.

        internal static string EnsureExtendedPrefix(string path)
        {
            if (IsPartiallyQualified(path) || IsDevice(path))
            {
                return path;
            }

            // Given \\server\share in longpath becomes \\?\UNC\server\share
            if (path.StartsWith(UncPathPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return path.Insert(2, UncDevicePrefixToInsert);
            }

            return ExtendedDevicePathPrefix + path;
        }

        private const string ExtendedDevicePathPrefix = @"\\?\";
        private const string UncPathPrefix = @"\\";
        private const string UncDevicePrefixToInsert = @"?\UNC\";
        private const string UncExtendedPathPrefix = @"\\?\UNC\";
        private const string DevicePathPrefix = @"\\.\";

        // \\?\, \\.\, \??\
        private const int DevicePrefixLength = 4;

        /// <summary>
        /// Returns true if the given character is a valid drive letter
        /// </summary>
        private static bool IsValidDriveChar(char value)
        {
            return ((value >= 'A' && value <= 'Z') || (value >= 'a' && value <= 'z'));
        }

        private static bool IsDevice(string path)
        {
            return IsExtended(path)
                ||
                (
                    path.Length >= DevicePrefixLength
                    && IsDirectorySeparator(path[0])
                    && IsDirectorySeparator(path[1])
                    && (path[2] == '.' || path[2] == '?')
                    && IsDirectorySeparator(path[3])
                );
        }

        private static bool IsExtended(string path)
        {
            return path.Length >= DevicePrefixLength
                && path[0] == '\\'
                && (path[1] == '\\' || path[1] == '?')
                && path[2] == '?'
                && path[3] == '\\';
        }

        /// <summary>
        /// Returns true if the path specified is relative to the current drive or working directory.
        /// Returns false if the path is fixed to a specific drive or UNC path.  This method does no
        /// validation of the path (URIs will be returned as relative as a result).
        /// </summary>
        /// <remarks>
        /// Handles paths that use the alternate directory separator.  It is a frequent mistake to
        /// assume that rooted paths (Path.IsPathRooted) are not relative.  This isn't the case.
        /// "C:a" is drive relative- meaning that it will be resolved against the current directory
        /// for C: (rooted, but relative). "C:\a" is rooted and not relative (the current directory
        /// will not be used to modify the path).
        /// </remarks>
        private static bool IsPartiallyQualified(string path)
        {
            if (path.Length < 2)
            {
                // It isn't fixed, it must be relative.  There is no way to specify a fixed
                // path with one character (or less).
                return true;
            }

            if (IsDirectorySeparator(path[0]))
            {
                // There is no valid way to specify a relative path with two initial slashes or
                // \? as ? isn't valid for drive relative paths and \??\ is equivalent to \\?\
                return !(path[1] == '?' || IsDirectorySeparator(path[1]));
            }

            // The only way to specify a fixed path that doesn't begin with two slashes
            // is the drive, colon, slash format- i.e. C:\
            return !((path.Length >= 3)
                && (path[1] == Path.VolumeSeparatorChar)
                && IsDirectorySeparator(path[2])
                // To match old behavior we'll check the drive character for validity as the path is technically
                // not qualified if you don't have a valid drive. "=:\" is the "=" file's default data stream.
                && IsValidDriveChar(path[0]));
        }
        /// <summary>
        /// True if the given character is a directory separator.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsDirectorySeparator(char c)
        {
            return c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;
        }
    }
}