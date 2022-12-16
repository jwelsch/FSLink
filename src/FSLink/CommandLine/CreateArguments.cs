using CommandLine;
using FSLinkCommand.Command.Create;
using FSLinkLib;

namespace FSLink.CommandLine
{
    [Verb("create", HelpText = "Creates a symbolic link, hard link, or junction.")]
    public class CreateArguments : ICreateArguments
    {
        [Option("link-type", Required = true, HelpText = "The type of link to create. Valid values include: SymbolicLink, HardLink, Junction")]
        public FileSystemLinkType LinkType { get; set; }

        [Option("link-path", Required = true, HelpText = "The path of the link in the file system.")]
        public string LinkPath { get; set; }

        [Option("target-path", Required = true, HelpText = "The path the link points to in the file system.")]
        public string TargetPath { get; set; }
    }
}
