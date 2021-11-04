using FSLinkCommon.Wraps;
using System;
using System.Threading.Tasks;

namespace FSLinkCommand.Command.Delete
{
    public class DeleteCommand : ICommandBase
    {
        private readonly IDeleteArguments _commandArguments;
        private readonly IFileWrap _fileWrap;
        private readonly IDirectoryWrap _directoryWrap;

        public string Name => "Delete";

        public DeleteCommand(IDeleteArguments commandArguments, IFileWrap fileWrap, IDirectoryWrap directoryWrap)
        {
            _commandArguments = commandArguments;
            _fileWrap = fileWrap;
            _directoryWrap = directoryWrap;
        }

        public async Task<ICommandResult> Run()
        {
            return await Task.Run(() =>
            {
                if (_fileWrap.IsDirectory(_commandArguments.Path))
                {
                    _directoryWrap.Delete(_commandArguments.Path);
                }
                else
                {
                    _fileWrap.Delete(_commandArguments.Path);
                }

                return new SuccessCommandResult(Name, _commandArguments.Path);
            });
        }
    }
}
