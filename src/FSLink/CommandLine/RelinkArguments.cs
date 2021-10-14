using CommandLine;
using FSLinkCommand.Relink;

namespace FSLink.CommandLine
{
    [Verb("relink")]
    public class RelinkArguments : IRelinkArguments
    {
        [Option("link-path", Required = true)]
        public string LinkPath { get; set; }

        [Option("new-target-path", Required = true)]
        public string NewTargetPath { get; set; }
    }
}
