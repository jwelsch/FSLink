using CommandLine;
using FSLinkCommand.Command.Scan;

namespace FSLink.CommandLine
{
    [Verb("scan")]
    public class ScanArguments : IScanArguments
    {
        [Option("path", Required = true)]
        public string Path { get; set; }

        [Option("recurse", Required = false)]
        public bool Recurse { get; set; }
    }
}
