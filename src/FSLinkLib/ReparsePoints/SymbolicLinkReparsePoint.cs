using FSLinkLib.PInvoke;

#nullable enable

namespace FSLinkLib.ReparsePoints
{
    public interface ISymbolicLinkReparsePoint : IReparsePoint
    {
        ushort SubstituteNameOffset { get; }

        ushort SubstituteNameLength { get; }

        ushort PrintNameOffset { get; }

        ushort PrintNameLength { get; }

        uint Flags { get; }
    }

    public class SymbolicLinkReparsePoint : ReparsePointBase, ISymbolicLinkReparsePoint
    {
        public ushort SubstituteNameOffset { get; }

        public ushort SubstituteNameLength { get; }

        public ushort PrintNameOffset { get; }

        public ushort PrintNameLength { get; }

        public uint Flags { get; }

        public SymbolicLinkReparsePoint(string path, System.IO.FileAttributes attributes, REPARSE_DATA_BUFFER_SYMBOLICLINK dataBuffer)
            : base(path, attributes, dataBuffer.ReparseTag, dataBuffer.ReparseDataLength, dataBuffer.Reserved, dataBuffer.PathBuffer, 0x3FF4) // 16 KB - 12 bytes
        {
            SubstituteNameOffset = dataBuffer.SubstituteNameOffset;
            SubstituteNameLength = dataBuffer.SubstituteNameLength;
            PrintNameOffset = dataBuffer.PrintNameOffset;
            PrintNameLength = dataBuffer.PrintNameLength;
            Flags = dataBuffer.Flags;
        }
    }
}
