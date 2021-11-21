using FSLinkCommand.Command;
using System.Threading.Tasks;

#nullable enable

namespace FSLinkCommand.Tests.Fakes
{
    public class FakeCommand : CommandBase<IFakeArguments>
    {
        public FakeCommand()
            : base("Fake")
        {
        }

        protected async override Task<ICommandResult> DoRun(IFakeArguments arguments)
        {
            return await Task.FromResult(new SuccessCommandResult(Name));
        }
    }
}
