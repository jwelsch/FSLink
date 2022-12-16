using CommandLine;
using FSLinkCommand.Command.Relink;

namespace FSLink.CommandLine
{
    [Verb("relink", HelpText = "Resets the target of a link to a new path.")]
    public class RelinkArguments : IRelinkArguments
    {
        [Option("link-path", Required = true, HelpText = "The path of the link in the file system.")]
        public string LinkPath { get; set; }

        [Option("new-target-path", Required = true, HelpText = "The path of the new target in the file system.")]
        public string NewTargetPath { get; set; }
    }
}
