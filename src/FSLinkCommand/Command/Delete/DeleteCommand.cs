using FSLinkCommon.Wraps;
using System.Threading.Tasks;

#nullable enable

namespace FSLinkCommand.Command.Delete
{
    public class DeleteCommand : CommandBase<IDeleteArguments>
    {
        private readonly IFileWrap _fileWrap;
        private readonly IDirectoryWrap _directoryWrap;

        public DeleteCommand(IFileWrap fileWrap, IDirectoryWrap directoryWrap)
            : base("Delete")
        {
            _fileWrap = fileWrap;
            _directoryWrap = directoryWrap;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected override async Task<ICommandResult> DoRun(IDeleteArguments arguments)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (_fileWrap.IsDirectory(arguments.Path))
            {
                _directoryWrap.Delete(arguments.Path);
            }
            else
            {
                _fileWrap.Delete(arguments.Path);
            }

            return new SuccessCommandResult(Name, arguments.Path);
        }
    }
}
