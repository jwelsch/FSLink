using FSLinkCommon.Wraps;
using FSLinkLib;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FSLinkCommand.Create
{
    public interface ICreateArguments : ICommandArguments
    {
        FileSystemLinkType LinkType { get; }

        string LinkPath { get; }

        string TargetPath { get; }
    }

    public class CreateArguments : ICreateArguments
    {
        public FileSystemLinkType LinkType { get; }

        public string LinkPath { get; }

        public string TargetPath { get; }

        public CreateArguments(FileSystemLinkType linkType, string linkPath, string targetPath)
        {
            LinkType = linkType;
            LinkPath = linkPath;
            TargetPath = targetPath;
        }
    }

    public class CreateCommand : ICommandBase
    {
        private readonly ICreateArguments _commandArguments;
        private readonly IFileSystemLink _fileSystemLink;
        private readonly IFileWrap _fileWrap;
        private readonly IDirectoryWrap _directoryWrap;

        public string Name => "Create";

        public CreateCommand(ICreateArguments commandArguments, IFileSystemLink fileSystemLink, IFileWrap fileWrap, IDirectoryWrap directoryWrap)
        {
            _commandArguments = commandArguments;
            _fileSystemLink = fileSystemLink;
            _fileWrap = fileWrap;
            _directoryWrap = directoryWrap;
        }

        public async Task<ICommandResult> Run()
        {
            //try
            //{
                return await Task.Run(() => _commandArguments.LinkType switch
                {
                    FileSystemLinkType.HardLink => CreateHardLink(_commandArguments.LinkPath, _commandArguments.TargetPath),
                    FileSystemLinkType.Junction => CreateJunction(_commandArguments.LinkPath, _commandArguments.TargetPath),
                    FileSystemLinkType.SymbolicLink => CreateSymbolicLink(_commandArguments.LinkPath, _commandArguments.TargetPath),
                    _ => new ErrorCommandResult(Name, new ArgumentException($"Unknown file system link type: '{_commandArguments.LinkType}'"))
                });
            //}
            //catch (Exception ex)
            //{
            //    return new ErrorCommandResult(Name, new IOException($"Failed to create {_commandArguments.LinkType} at path '{_commandArguments.LinkPath}' with target '{_commandArguments.TargetPath}'", ex));
            //}
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
