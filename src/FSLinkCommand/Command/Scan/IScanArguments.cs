namespace FSLinkCommand.Command.Scan
{
    public interface IScanArguments : ICommandArguments
    {
        public string Path { get; }

        public bool Recurse { get; }
    }
}
