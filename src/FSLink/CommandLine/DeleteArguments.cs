using CommandLine;
using FSLinkCommand.Command.Delete;

namespace FSLink.CommandLine
{
    [Verb("delete", HelpText = "Deletes a symbolic link, hard link, or junction.")]
    public class DeleteArguments : IDeleteArguments
    {
        [Option("path", Required = true, HelpText = "The path of the link to delete.")]
        public string Path { get; set; }
    }
}
