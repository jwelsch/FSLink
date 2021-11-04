namespace FSLinkCommand.Command.Reparse
{
    public interface IReparseArguments : ICommandArguments
    {
        string Path { get; }
    }
}
