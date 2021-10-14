using System.Threading.Tasks;

namespace FSLinkCommand
{
    public interface ICommandBase
    {
        string Name { get; }

        Task<ICommandResult> Run();
    }
}
