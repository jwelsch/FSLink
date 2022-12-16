using CommandLine;
using FSLinkCommand.Command.Scan;

namespace FSLink.CommandLine
{
    [Verb("scan", HelpText = "Scans a directory for file system links and prints the results to the console.")]
    public class ScanArguments : IScanArguments
    {
        [Option("path", Required = true, HelpText = "Path of the directory to scan.")]
        public string Path { get; set; }

        [Option("recurse", Required = false, HelpText = "Include to recurse through child directories, exclude to only scan the top level directory.")]
        public bool Recurse { get; set; }
    }
}
