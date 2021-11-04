using CommandLine;
using FSLinkCommand.Command.Create;
using FSLinkLib;

namespace FSLink.CommandLine
{
    [Verb("create")]
    public class CreateArguments : ICreateArguments
    {
        [Option("link-type", Required = true)]
        public FileSystemLinkType LinkType { get; set; }

        [Option("link-path", Required = true)]
        public string LinkPath { get; set; }

        [Option("target-path", Required = true)]
        public string TargetPath { get; set; }
    }
}
