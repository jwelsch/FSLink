using FSLinkCommand.Create;
using FSLinkCommand.Delete;
using FSLinkCommon.Wraps;
using FSLinkLib;
using System;
using System.Threading.Tasks;

namespace FSLinkCommand.Relink
{
    public interface IRelinkArguments : ICommandArguments
    {
        string LinkPath { get; }

        string NewTargetPath { get; }
    }

    public class RelinkCommand : ICommandBase
    {
        private readonly IRelinkArguments _commandArguments;
        private readonly IFileSystemLink _fileSystemLink;
        private readonly IFileWrap _fileWrap;
        private readonly IDirectoryWrap _directoryWrap;

        public string Name => "Relink";

        public RelinkCommand(IRelinkArguments commandArguments, IFileSystemLink fileSystemLink, IFileWrap fileWrap, IDirectoryWrap directoryWrap)
        {
            _commandArguments = commandArguments;
            _fileSystemLink = fileSystemLink;
            _fileWrap = fileWrap;
            _directoryWrap = directoryWrap;
        }

        public async Task<ICommandResult> Run()
        {
            try
            {
                var linkType = _fileSystemLink.GetLinkType(_commandArguments.LinkPath);

                if (linkType == FileSystemLinkType.None)
                {
                    throw new ArgumentException($"The path '{_commandArguments.LinkPath}' does not point to a file system link entity (symbolic link, hard link, or junction).");
                }

                var oldTargetPath = _fileSystemLink.GetLinkTarget(_commandArguments.LinkPath);

                var deleteCommand = new DeleteCommand(new DeleteArguments { Path = _commandArguments.LinkPath }, _fileWrap, _directoryWrap);
                var deleteResult = await deleteCommand.Run();

                if (!deleteResult.Success)
                {
                    return new ErrorCommandResult(Name, deleteResult.Error);
                }

                var createCommand = new CreateCommand(new CreateArguments(linkType, _commandArguments.LinkPath, _commandArguments.NewTargetPath), _fileSystemLink, _fileWrap, _directoryWrap);
                var createResult = await createCommand.Run();

                if (!createResult.Success)
                {
                    return new ErrorCommandResult(Name, createResult.Error);
                }

                return new SuccessCommandResult(Name, new[] { _commandArguments.LinkPath, oldTargetPath, _commandArguments.NewTargetPath });
            }
            catch (Exception ex)
            {
                return new ErrorCommandResult(Name, ex);
            }
        }
    }
}
