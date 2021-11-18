using FSLinkCommand.Command.Create;
using FSLinkCommand.Command.Delete;
using FSLinkCommand.Command.Relink;
using FSLinkCommand.Command.Reparse;
using FSLinkCommand.Command.Scan;
using FSLinkCommand.FileSystem;
using FSLinkCommon.Wraps;
using FSLinkLib;
using System;

#nullable enable

namespace FSLinkCommand.Command
{
    public interface ICommandFactory
    {
        ICommandBase Create(ICommandArguments commandArguments);
    }

    public class CommandFactory : ICommandFactory
    {
        private readonly IFileSystemLink _fileSystemLink;
        private readonly IFileSystemScanner _fileSystemScanner;
        private readonly IFileWrap _fileWrap;
        private readonly IDirectoryWrap _directoryWrap;
        private readonly IScanOutput _scanOutput;
        private readonly ILogReparseOutputFactory _logReparseOutputFactory;
        private readonly IHardLink _hardLink;
        private readonly IJunction _junction;
        private readonly ISymbolicLink _symbolicLink;

        public CommandFactory(IFileSystemLink fileSystemLink, IFileSystemScanner fileSystemScanner, IFileWrap fileWrap, IDirectoryWrap directoryWrap, IScanOutput scanOutput, ILogReparseOutputFactory logReparseOutputFactory, IHardLink hardLink, IJunction junction, ISymbolicLink symbolicLink)
        {
            _fileSystemLink = fileSystemLink;
            _fileSystemScanner = fileSystemScanner;
            _fileWrap = fileWrap;
            _directoryWrap = directoryWrap;
            _scanOutput = scanOutput;
            _logReparseOutputFactory = logReparseOutputFactory;
            _hardLink = hardLink;
            _junction = junction;
            _symbolicLink = symbolicLink;
        }

        public ICommandBase Create(ICommandArguments commandArguments)
        {
            return commandArguments switch
            {
                IScanArguments _ => new ScanCommand(_fileSystemLink, _fileSystemScanner, _scanOutput),
                ICreateArguments _ => new CreateCommand(_fileSystemLink, _fileWrap, _directoryWrap),
                IDeleteArguments _ => new DeleteCommand(_fileWrap, _directoryWrap),
                IRelinkArguments _ => new RelinkCommand(_hardLink, _junction, _symbolicLink, _fileSystemLink),
                IReparseArguments _ => new ReparseCommand(_logReparseOutputFactory, _fileSystemLink),
                _ => throw new ArgumentException($"Unknown command argument type '{commandArguments.GetType()}'")
            };
        }
    }
}
