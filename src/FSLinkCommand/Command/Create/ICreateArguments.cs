using FSLinkLib;

namespace FSLinkCommand.Command.Create
{
    public interface ICreateArguments : ICommandArguments
    {
        FileSystemLinkType LinkType { get; }

        string LinkPath { get; }

        string TargetPath { get; }
    }
}
