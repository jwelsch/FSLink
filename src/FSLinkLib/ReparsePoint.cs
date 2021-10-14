#nullable enable

using System;
using System.IO;
using System.Linq;

namespace FSLinkLib
{
    public interface IReparsePoint
    {
        string Path { get; }

        FileAttributes Attributes { get; }

        bool IsDirectory { get; }

        IReparseTag Tag { get; }

        string TagName { get; }
    }

    public class ReparsePoint : IReparsePoint
    {
        public string Path { get; }

        public FileAttributes Attributes { get; }

        public bool IsDirectory => (Attributes & FileAttributes.Directory) == FileAttributes.Directory;

        public IReparseTag Tag { get; }

        public string TagName { get; }

        public ReparsePoint(string path, FileAttributes attributes, IReparseTag tag)
        {
            Path = path;
            Attributes = attributes;
            Tag = tag;
            TagName = LookUpTagName(Tag);
        }

        private static string LookUpTagName(IReparseTag reparseTag)
        {
            var tagValues = Enum.GetValues<ReparseTagValues>();

            var tagValue = tagValues.FirstOrDefault(i => ((uint)i & ReparseTag.TagValueMask) == reparseTag.TagValue);

            var name = Enum.GetName(tagValue);

            return name ?? throw new ArgumentException($"Unknown value '{tagValue}' for {nameof(ReparseTagValues)}.");
        }
    }
}
