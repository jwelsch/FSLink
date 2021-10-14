using FSLinkLib.PInvoke;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

#nullable enable

namespace FSLinkLib
{
    //
    // Based on:
    // https://github.com/PowerShell/PowerShell/blob/cfe45defc1ed87a7a7b916f154e56124d81a320b/src/System.Management.Automation/namespaces/FileSystemProvider.cs#L8667
    //

    /// <summary>
    /// Represents alternate stream data retrieved from a file.
    /// </summary>
    public class AlternateStreamData
    {
        /// <summary>
        /// The name of the file that holds this stream.
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// The name of this stream.
        /// </summary>
        public string? Stream { get; set; }

        /// <summary>
        /// The length of this stream.
        /// </summary>
        public long Length { get; set; }
    }

    /// <summary>
    /// Provides access to alternate data streams on a file.
    /// </summary>
    public static class AlternateDataStreamUtilities
    {
        /// <summary>
        /// List all of the streams on a file.
        /// </summary>
        /// <param name="path">The fully-qualified path to the file.</param>
        /// <returns>The list of streams (and their size) in the file.</returns>
        internal static List<AlternateStreamData> GetStreams(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            List<AlternateStreamData> alternateStreams = new();

            AlternateStreamNativeData findStreamData = new();

            var handle = NativeMethods.FindFirstStreamW(path,
                                                        NativeMethods.StreamInfoLevels.FindStreamInfoStandard,
                                                        findStreamData,
                                                        0);

            if (handle.IsInvalid)
            {
                var error = Marshal.GetLastWin32Error();

                // Directories don't normally have alternate streams, so this is not an exceptional state.
                // If a directory has no alternate data streams, FindFirstStreamW returns ERROR_HANDLE_EOF.
                if (error == NativeMethods.ERROR_HANDLE_EOF)
                {
                    return alternateStreams;
                }

                // An unexpected error was returned, that we don't know how to interpret. The most helpful
                // thing we can do at this point is simply throw the raw Win32 exception.
                throw new Win32Exception(error);
            }

            if (findStreamData == null)
            {
                throw new InvalidOperationException($"Stream data not found. Path: {path}");
            }

            if (string.IsNullOrEmpty(findStreamData.Name))
            {
                throw new InvalidOperationException($"Stream data does not have a name. Path: {path}");
            }

            try
            {
                do
                {
                    // Remove the leading ':'
                    findStreamData.Name = findStreamData.Name![1..];

                    // And trailing :$DATA (as long as it's not the default data stream)
                    const string dataStream = ":$DATA";
                    if (!string.Equals(findStreamData.Name, dataStream, StringComparison.OrdinalIgnoreCase))
                    {
                        findStreamData.Name = findStreamData.Name.Replace(dataStream, string.Empty);
                    }

                    var data = new AlternateStreamData();
                    data.Stream = findStreamData.Name;
                    data.Length = findStreamData.Length;
                    data.FileName = path.Replace(data.Stream, string.Empty);
                    data.FileName = data.FileName.Trim(':');

                    alternateStreams.Add(data);
                    findStreamData = new AlternateStreamNativeData();
                }
                while (NativeMethods.FindNextStreamW(handle, findStreamData));

                var lastError = Marshal.GetLastWin32Error();
                if (lastError != NativeMethods.ERROR_HANDLE_EOF)
                {
                    throw new Win32Exception(lastError);
                }
            }
            finally
            {
                handle.Dispose();
            }

            return alternateStreams;
        }

        /// <summary>
        /// Creates a file stream on a file.
        /// </summary>
        /// <param name="path">The fully-qualified path to the file.</param>
        /// <param name="streamName">The name of the alternate data stream to open.</param>
        /// <param name="mode">The FileMode of the file.</param>
        /// <param name="access">The FileAccess of the file.</param>
        /// <param name="share">The FileShare of the file.</param>
        /// <returns>A FileStream that can be used to interact with the file.</returns>
        internal static FileStream? CreateFileStream(string path, string streamName, FileMode mode, FileAccess access, FileShare share)
        {
            if (!TryCreateFileStream(path, streamName, mode, access, share, out var stream))
            {
                var errorMessage = $"Alternate data stream not found in stream '{streamName}' with path '{path}'";
                throw new FileNotFoundException(errorMessage, $"{path}:{streamName}");
            }

            return stream;
        }

        /// <summary>
        /// Tries to create a file stream on a file.
        /// </summary>
        /// <param name="path">The fully-qualified path to the file.</param>
        /// <param name="streamName">The name of the alternate data stream to open.</param>
        /// <param name="mode">The FileMode of the file.</param>
        /// <param name="access">The FileAccess of the file.</param>
        /// <param name="share">The FileShare of the file.</param>
        /// <param name="stream">A FileStream that can be used to interact with the file.</param>
        /// <returns>True if the stream was successfully created, otherwise false.</returns>
        internal static bool TryCreateFileStream(string path, string streamName, FileMode mode, FileAccess access, FileShare share, out FileStream? stream)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (streamName == null)
            {
                throw new ArgumentNullException(nameof(streamName));
            }

            if (mode == FileMode.Append)
            {
                mode = FileMode.OpenOrCreate;
            }

            var resultPath = $"{path}:{streamName}";
            var handle = NativeMethods.CreateFile(resultPath, access, share, IntPtr.Zero, mode, 0, IntPtr.Zero);

            if (handle.IsInvalid)
            {
                stream = null;
                return false;
            }

            stream = new FileStream(handle, access);
            return true;
        }

        /// <summary>
        /// Removes an alternate data stream.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="streamName">The name of the alternate data stream to delete.</param>
        internal static void DeleteFileStream(string path, string streamName)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (streamName == null)
            {
                throw new ArgumentNullException(nameof(streamName));
            }

            var adjustedStreamName = streamName.Trim();
            if (adjustedStreamName.IndexOf(':') != 0)
            {
                adjustedStreamName = ":" + adjustedStreamName;
            }

            var resultPath = path + adjustedStreamName;

            File.Delete(resultPath);
        }

        internal static class NativeMethods
        {
            internal const int ERROR_HANDLE_EOF = 38;
            internal const int ERROR_INVALID_PARAMETER = 87;

            internal enum StreamInfoLevels { FindStreamInfoStandard = 0 }

            [DllImport(PinvokeDllNames.CreateFileDllName, CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern SafeFileHandle CreateFile(string lpFileName,
                FileAccess dwDesiredAccess, FileShare dwShareMode,
                IntPtr lpSecurityAttributes, FileMode dwCreationDisposition,
                int dwFlagsAndAttributes, IntPtr hTemplateFile);

            [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern SafeFindHandle FindFirstStreamW(
                string lpFileName, StreamInfoLevels InfoLevel,
                [In, Out, MarshalAs(UnmanagedType.LPStruct)]
                AlternateStreamNativeData lpFindStreamData, uint dwFlags);

            [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool FindNextStreamW(
                SafeFindHandle hndFindFile,
                [In, Out, MarshalAs(UnmanagedType.LPStruct)]
                AlternateStreamNativeData lpFindStreamData);
        }

        internal sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeFindHandle() : base(true) { }

            protected override bool ReleaseHandle()
            {
                return FindClose(this.handle);
            }

            [DllImport(PinvokeDllNames.FindCloseDllName)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool FindClose(IntPtr handle);
        }

        /// <summary>
        /// Represents alternate stream data retrieved from a file.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal class AlternateStreamNativeData
        {
            /// <summary>
            /// The length of this stream.
            /// </summary>
            public long Length;

            /// <summary>
            /// The name of this stream.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 296)]
            public string? Name;
        }
    }
}
