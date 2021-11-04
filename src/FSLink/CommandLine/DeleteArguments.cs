using CommandLine;
using FSLinkCommand.Command.Delete;

namespace FSLink.CommandLine
{
    [Verb("delete")]
    public class DeleteArguments : IDeleteArguments
    {
        [Option("path", Required = true)]
        public string Path { get; set; }
    }
}
