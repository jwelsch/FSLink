namespace FSLinkCommand.Command.Relink
{
    public interface IRelinkArguments : ICommandArguments
    {
        string LinkPath { get; }

        string NewTargetPath { get; }
    }
}
