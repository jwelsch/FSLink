using FSLinkCommand.FileSystem;
using FSLinkLib;
using System.Threading.Tasks;

namespace FSLinkCommand.Command.Relink
{
    public class RelinkCommand : CommandBase<IRelinkArguments>
    {
        private readonly IHardLink _hardLink;
        private readonly IJunction _junction;
        private readonly ISymbolicLink _symbolicLink;
        private readonly IFileSystemLink _fileSystemLink;

        public RelinkCommand(IHardLink hardLink, IJunction junction, ISymbolicLink symbolicLink, IFileSystemLink fileSystemLink)
            : base("Relink")
        {
            _hardLink = hardLink;
            _junction = junction;
            _symbolicLink = symbolicLink;
            _fileSystemLink = fileSystemLink;
        }

        protected override async Task<ICommandResult> DoRun(IRelinkArguments arguments)
        {
            return await Task.Run(() =>
            {
                var linkType = _fileSystemLink.GetLinkType(arguments.LinkPath);

                return linkType switch
                {
                    FileSystemLinkType.HardLink => DoHardLink(arguments),
                    FileSystemLinkType.Junction => DoJunction(arguments),
                    FileSystemLinkType.SymbolicLink => DoSymbolicLink(arguments),
                    _ => new ErrorCommandResult(Name, $"The path '{arguments.LinkPath}' does not point to a file system link entity (symbolic link, hard link, or junction).")
                };
            });
        }

        private ICommandResult DoHardLink(IRelinkArguments arguments)
        {
            _hardLink.Delete(arguments.LinkPath);
            _hardLink.Create(arguments.LinkPath, arguments.NewTargetPath);
            return new SuccessCommandResult(Name, new[] { arguments.LinkPath, arguments.NewTargetPath });
        }

        private ICommandResult DoJunction(IRelinkArguments arguments)
        {
            _junction.Unlink(arguments.LinkPath);
            _junction.Create(arguments.LinkPath, arguments.NewTargetPath);
            return new SuccessCommandResult(Name, new[] { arguments.LinkPath, arguments.NewTargetPath });
        }

        private ICommandResult DoSymbolicLink(IRelinkArguments arguments)
        {
            _symbolicLink.Unlink(arguments.LinkPath);
            _symbolicLink.Create(arguments.LinkPath, arguments.NewTargetPath);
            return new SuccessCommandResult(Name, new[] { arguments.LinkPath, arguments.NewTargetPath });
        }
    }
}
