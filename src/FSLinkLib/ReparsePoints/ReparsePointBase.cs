using System;
using System.IO;
using System.Linq;

#nullable enable

namespace FSLinkLib.ReparsePoints
{
    public interface IReparsePoint
    {
        string Path { get; }

        FileAttributes Attributes { get; }

        bool IsDirectory { get; }

        IReparseTag Tag { get; }

        string TagName { get; }

        uint DataLength { get; }

        uint Reserved { get; }

        byte[] Data { get; }

        int MaxDataLength { get; }
    }

    public abstract class ReparsePointBase : IReparsePoint
    {
        public string Path { get; }

        public FileAttributes Attributes { get; }

        public bool IsDirectory => (Attributes & FileAttributes.Directory) == FileAttributes.Directory;

        public IReparseTag Tag { get; }

        public string TagName { get; }

        public uint DataLength { get; }

        public uint Reserved { get; }

        public byte[] Data { get; }

        public int MaxDataLength { get; }

        protected ReparsePointBase(string path, FileAttributes attributes, uint reparseTag, uint dataLength, uint reserved, byte[] data, int maxDataLength)
        {
            Path = path;
            Attributes = attributes;
            Tag = new ReparseTag(reparseTag);
            TagName = LookUpTagName(Tag);
            DataLength = dataLength;
            Reserved = reserved;
            Data = data;
            MaxDataLength = maxDataLength;
        }

        public static string LookUpTagName(IReparseTag reparseTag)
        {
            var tagValues = Enum.GetValues<ReparseTagValues>();
            var tagValue = tagValues.FirstOrDefault(i => ((uint)i & ReparseTag.TagValueMask) == reparseTag.TagValue);
            var name = Enum.GetName(tagValue);

            return name ?? throw new ArgumentException($"Unknown value '{tagValue}' for {nameof(ReparseTagValues)}.");
        }

        public static ReparseTagValues LookUpTag(IReparseTag reparseTag)
        {
            var tagValues = Enum.GetValues<ReparseTagValues>();
            var tagValue = tagValues.FirstOrDefault(i => ((uint)i & ReparseTag.TagValueMask) == reparseTag.TagValue);

            return tagValue != ReparseTagValues.IO_REPARSE_NONE
                             ? tagValue
                             : throw new ArgumentException($"Unknown value '{reparseTag.TagValue}' for {nameof(ReparseTagValues)}.");
        }
    }
}
