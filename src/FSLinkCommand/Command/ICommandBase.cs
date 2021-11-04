using System.Threading.Tasks;

namespace FSLinkCommand.Command
{
    public interface ICommandBase
    {
        string Name { get; }

        Task<ICommandResult> Run();
    }
}
