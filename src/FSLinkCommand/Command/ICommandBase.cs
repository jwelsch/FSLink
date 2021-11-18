using System;
using System.Threading.Tasks;

namespace FSLinkCommand.Command
{
    public interface ICommandBase
    {
        string Name { get; }

        Task<ICommandResult> Run(ICommandArguments arguments);
    }

    public abstract class CommandBase<T> : ICommandBase where T : ICommandArguments
    {
        public string Name { get; }

        protected CommandBase(string name)
        {
            Name = name;
        }

        public async Task<ICommandResult> Run(ICommandArguments arguments)
        {
            if (arguments is not T castArguments)
            {
                throw new ArgumentException($"Cannot cast to type '{typeof(T)}'", nameof(arguments));
            }

            return await DoRun(castArguments);
        }

        protected abstract Task<ICommandResult> DoRun(T arguments);
    }
}
