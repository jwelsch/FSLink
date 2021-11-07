using FSLinkLib;
using System.IO;
using System.Threading.Tasks;

#nullable enable

namespace FSLinkCommand.Command.Reparse
{
    public class ReparseCommand : ICommandBase
    {
        private readonly IReparseArguments _commandArguments;
        private readonly ILogReparseOutputFactory _logReparseOutputFactory;
        private readonly IFileSystemLink _fileSystemLink;

        public string Name => "Reparse";

        public ReparseCommand(IReparseArguments commandArguments, ILogReparseOutputFactory logReparseOutputFactory, IFileSystemLink fileSystemLink)
        {
            _commandArguments = commandArguments;
            _fileSystemLink = fileSystemLink;
            _logReparseOutputFactory = logReparseOutputFactory;
        }

        public async Task<ICommandResult> Run()
        {
            return await Task.Run(() =>
            {
                var reparsePoint = _fileSystemLink.GetReparsePoint(_commandArguments.Path);

                if (reparsePoint == null)
                {
                    return new ErrorCommandResult(Name, new IOException($"Failed to get reparse point data for path '{_commandArguments.Path}'"));
                }

                var output = _logReparseOutputFactory.Create(reparsePoint);

                output.OnReparsePointData(reparsePoint);

                return (ICommandResult)new SuccessCommandResult(Name, reparsePoint);
            });
        }
    }
}
