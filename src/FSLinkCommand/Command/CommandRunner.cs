using System;
using System.IO;
using System.Threading.Tasks;

#nullable enable

namespace FSLinkCommand.Command
{
    public interface ICommandRunner
    {
        ICommandResult Execute(ICommandBase command, ICommandArguments arguments);
    }

    public class CommandRunner : ICommandRunner
    {
        public ICommandResult Execute(ICommandBase command, ICommandArguments arguments)
        {
            try
            {
                var task = Task.Run(() => command.Run(arguments));
                var awaiter = task.GetAwaiter();

                return awaiter.GetResult();
            }
            catch (Exception ex)
            {
                return new ErrorCommandResult(command.Name, new IOException($"Command '{command.Name}' threw an exception.", ex));
            }
        }
    }
}
