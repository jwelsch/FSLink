using FSLinkLib.PInvoke;

#nullable enable

namespace FSLinkLib.ReparsePoints
{
    public interface IDataReparsePoint : IReparsePoint
    {
    }

    public class DataReparsePoint : ReparsePointBase, IDataReparsePoint
    {
        public DataReparsePoint(string path, System.IO.FileAttributes attributes, REPARSE_DATA_BUFFER dataBuffer)
            : base(path, attributes, dataBuffer.ReparseTag, dataBuffer.ReparseDataLength, dataBuffer.Reserved, dataBuffer.DataBuffer, 0x4000) // 16 KB
        {
        }
    }
}
