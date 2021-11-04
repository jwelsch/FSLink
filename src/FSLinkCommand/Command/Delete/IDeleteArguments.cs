namespace FSLinkCommand.Command.Delete
{
    public interface IDeleteArguments : ICommandArguments
    {
        string Path { get; }
    }
}
