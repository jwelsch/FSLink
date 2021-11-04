using CommandLine;
using FSLinkCommand.Command.Reparse;

namespace FSLink.CommandLine
{
    [Verb("reparse")]
    public class ReparseArguments : IReparseArguments
    {
        [Option("path", Required = true)]
        public string Path { get; set; }
    }
}
