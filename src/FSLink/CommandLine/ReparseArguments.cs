using CommandLine;
using FSLinkCommand.Command.Reparse;

namespace FSLink.CommandLine
{
    [Verb("reparse", HelpText = "Displays information in the console about a reparse point attached to the item with the specified path.")]
    public class ReparseArguments : IReparseArguments
    {
        [Option("path", Required = true, HelpText = "Path to the file system item with an attached reparse point.")]
        public string Path { get; set; }
    }
}
