using FSLinkCommon.Wraps;
using FSLinkLib;
using System;
using System.IO;
using System.Threading.Tasks;

#nullable enable

namespace FSLinkCommand.Command.Create
{
    public class CreateCommand : CommandBase<ICreateArguments>
    {
        private readonly IFileSystemLink _fileSystemLink;
        private readonly IFileWrap _fileWrap;
        private readonly IDirectoryWrap _directoryWrap;

        public CreateCommand(IFileSystemLink fileSystemLink, IFileWrap fileWrap, IDirectoryWrap directoryWrap)
            : base("Create")
        {
            _fileSystemLink = fileSystemLink;
            _fileWrap = fileWrap;
            _directoryWrap = directoryWrap;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected override async Task<ICommandResult> DoRun(ICreateArguments arguments)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return arguments.LinkType switch
            {
                FileSystemLinkType.HardLink => CreateHardLink(arguments.LinkPath, arguments.TargetPath),
                FileSystemLinkType.Junction => CreateJunction(arguments.LinkPath, arguments.TargetPath),
                FileSystemLinkType.SymbolicLink => CreateSymbolicLink(arguments.LinkPath, arguments.TargetPath),
                _ => new ErrorCommandResult(Name, new ArgumentException($"Unknown file system link type: '{arguments.LinkType}'"))
            };
        }

        private ICommandResult CreateHardLink(string hardLinkPath, string targetPath)
        {
            _fileSystemLink.CreateHardLink(hardLinkPath, targetPath);
            return new SuccessCommandResult(Name, new[] { hardLinkPath, targetPath });
        }

        private ICommandResult CreateJunction(string junctionPath, string targetPath)
        {
            if (!_directoryWrap.Exists(junctionPath))
            {
                _directoryWrap.CreateDirectory(junctionPath);
            }

            _fileSystemLink.CreateJunction(junctionPath, targetPath);
            return new SuccessCommandResult(Name, new[] { junctionPath, targetPath });
        }

        private ICommandResult CreateSymbolicLink(string symbolicLinkPath, string targetPath)
        {
            var attributes = _fileWrap.GetAttributes(targetPath);
            var symbolicLinkType = (attributes & FileAttributes.Directory) == FileAttributes.Directory ? SymbolicLinkType.Directory : SymbolicLinkType.File;

            _fileSystemLink.CreateSymbolicLink(symbolicLinkPath, targetPath, symbolicLinkType);
            return new SuccessCommandResult(Name, new[] { symbolicLinkPath, targetPath });
        }
    }
}
