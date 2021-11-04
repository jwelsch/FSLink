using FSLinkCommand.FileSystem;
using FSLinkLib;
using System.Threading.Tasks;

namespace FSLinkCommand.Command.Relink
{
    public class RelinkCommand : ICommandBase
    {
        private readonly IRelinkArguments _commandArguments;
        private readonly IHardLink _hardLink;
        private readonly IJunction _junction;
        private readonly ISymbolicLink _symbolicLink;
        private readonly IFileSystemLink _fileSystemLink;

        public string Name => "Relink";

        public RelinkCommand(IRelinkArguments commandArguments, IHardLink hardLink, IJunction junction, ISymbolicLink symbolicLink, IFileSystemLink fileSystemLink)
        {
            _commandArguments = commandArguments;
            _hardLink = hardLink;
            _junction = junction;
            _symbolicLink = symbolicLink;
            _fileSystemLink = fileSystemLink;
        }

        public async Task<ICommandResult> Run()
        {
            return await Task.Run(() =>
            {
                var linkType = _fileSystemLink.GetLinkType(_commandArguments.LinkPath);

                return linkType switch
                {
                    FileSystemLinkType.HardLink => DoHardLink(),
                    FileSystemLinkType.Junction => DoJunction(),
                    FileSystemLinkType.SymbolicLink => DoSymbolicLink(),
                    FileSystemLinkType.None => new ErrorCommandResult(Name, $"The path '{_commandArguments.LinkPath}' does not point to a file system link entity (symbolic link, hard link, or junction)."),
                    _ => new ErrorCommandResult(Name, $"The path '{_commandArguments.LinkPath}' is an unknown link type '{linkType}'.")
                };
            });
        }

        private ICommandResult DoHardLink()
        {
            _hardLink.Delete(_commandArguments.LinkPath);
            _hardLink.Create(_commandArguments.LinkPath, _commandArguments.NewTargetPath);
            return new SuccessCommandResult(Name, new[] { _commandArguments.LinkPath, _commandArguments.NewTargetPath });
        }

        private ICommandResult DoJunction()
        {
            _junction.Unlink(_commandArguments.LinkPath);
            _junction.Create(_commandArguments.LinkPath, _commandArguments.NewTargetPath);
            return new SuccessCommandResult(Name, new[] { _commandArguments.LinkPath, _commandArguments.NewTargetPath });
        }

        private ICommandResult DoSymbolicLink()
        {
            _symbolicLink.Unlink(_commandArguments.LinkPath);
            _symbolicLink.Create(_commandArguments.LinkPath, _commandArguments.NewTargetPath);
            return new SuccessCommandResult(Name, new[] { _commandArguments.LinkPath, _commandArguments.NewTargetPath });
        }
    }
}
