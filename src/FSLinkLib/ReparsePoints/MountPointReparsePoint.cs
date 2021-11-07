using FSLinkLib.PInvoke;

#nullable enable

namespace FSLinkLib.ReparsePoints
{
    public interface IMountPointReparsePoint : IReparsePoint
    {
        ushort SubstituteNameOffset { get; }

        ushort SubstituteNameLength { get; }

        ushort PrintNameOffset { get; }

        ushort PrintNameLength { get; }
    }

    public class MountPointReparsePoint : ReparsePointBase, IMountPointReparsePoint
    {
        public ushort SubstituteNameOffset { get; }

        public ushort SubstituteNameLength { get; }

        public ushort PrintNameOffset { get; }

        public ushort PrintNameLength { get; }

        public MountPointReparsePoint(string path, System.IO.FileAttributes attributes, REPARSE_DATA_BUFFER_MOUNTPOINT dataBuffer)
            : base(path, attributes, dataBuffer.ReparseTag, dataBuffer.ReparseDataLength, dataBuffer.Reserved, dataBuffer.PathBuffer, 0x3FF8) // 16 KB - 8 bytes
        {
            SubstituteNameOffset = dataBuffer.SubstituteNameOffset;
            SubstituteNameLength = dataBuffer.SubstituteNameLength;
            PrintNameOffset = dataBuffer.PrintNameOffset;
            PrintNameLength = dataBuffer.PrintNameLength;
        }
    }
}
