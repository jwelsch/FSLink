using FSLinkLib;
using System.IO;
using System.Threading.Tasks;

#nullable enable

namespace FSLinkCommand.Command.Reparse
{
    public class ReparseCommand : CommandBase<IReparseArguments>
    {
        private readonly ILogReparseOutputFactory _logReparseOutputFactory;
        private readonly IFileSystemLink _fileSystemLink;

        public ReparseCommand(ILogReparseOutputFactory logReparseOutputFactory, IFileSystemLink fileSystemLink)
            : base("Reparse")
        {
            _fileSystemLink = fileSystemLink;
            _logReparseOutputFactory = logReparseOutputFactory;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected override async Task<ICommandResult> DoRun(IReparseArguments arguments)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var reparsePoint = _fileSystemLink.GetReparsePoint(arguments.Path);

            if (reparsePoint == null)
            {
                return new ErrorCommandResult(Name, new IOException($"Failed to get reparse point data for path '{arguments.Path}'"));
            }

            var output = _logReparseOutputFactory.Create(reparsePoint);

            output.OnReparsePointData(reparsePoint);

            return new SuccessCommandResult(Name, reparsePoint);
        }
    }
}
