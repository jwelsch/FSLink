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

        /// <summary>
        /// Length of reparse point data, including all data after Reserved.
        /// </summary>
        uint DataLength { get; }

        uint Reserved { get; }

        byte[] Data { get; }

        /// <summary>
        /// Maximum size of a reparse point (16 KB).
        /// </summary>
        uint ReparsePointMaxLength { get; }

        /// <summary>
        /// Maximum size of the data buffer for this type of reparse point.
        /// </summary>
        uint MaxDataBufferLength { get; }

        /// <summary>
        /// Calculated length of data buffer for this reparse point.
        /// DataBufferLength = DataLength - (ReparsePointMaxLength - MaxDataBufferLength)
        /// </summary>
        uint DataBufferLength { get; }
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

        public uint ReparsePointMaxLength => 16384; // 16 KB

        public uint MaxDataBufferLength { get; }

        public uint DataBufferLength => DataLength - (ReparsePointMaxLength - MaxDataBufferLength);

        protected ReparsePointBase(string path, FileAttributes attributes, uint reparseTag, uint dataLength, uint reserved, byte[] data, uint maxDataBufferLength)
        {
            Path = path;
            Attributes = attributes;
            Tag = new ReparseTag(reparseTag);
            TagName = LookUpTagName(Tag);
            DataLength = dataLength;
            Reserved = reserved;
            Data = data;
            MaxDataBufferLength = maxDataBufferLength;
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
