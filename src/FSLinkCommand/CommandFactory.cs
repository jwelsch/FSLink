using FSLinkCommand.Command.Create;
using FSLinkCommand.Command.Delete;
using FSLinkCommand.Command.Relink;
using FSLinkCommand.Command.Reparse;
using FSLinkCommand.Command.Scan;
using FSLinkCommand.FileSystem;
using FSLinkCommon.Wraps;
using FSLinkLib;
using System;

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
        private readonly IReparseOutput _reparseOutput;
        private readonly IHardLink _hardLink;
        private readonly IJunction _junction;
        private readonly ISymbolicLink _symbolicLink;

        public CommandFactory(IFileSystemLink fileSystemLink, IFileSystemScanner fileSystemScanner, IFileWrap fileWrap, IDirectoryWrap directoryWrap, IScanOutput scanOutput, IReparseOutput reparseOutput, IHardLink hardLink, IJunction junction, ISymbolicLink symbolicLink)
        {
            _fileSystemLink = fileSystemLink;
            _fileSystemScanner = fileSystemScanner;
            _fileWrap = fileWrap;
            _directoryWrap = directoryWrap;
            _scanOutput = scanOutput;
            _reparseOutput = reparseOutput;
            _hardLink = hardLink;
            _junction = junction;
            _symbolicLink = symbolicLink;
        }

        public ICommandBase Create(ICommandArguments commandArguments)
        {
            return commandArguments switch
            {
                IScanArguments a => new ScanCommand(a, _fileSystemLink, _fileSystemScanner, _scanOutput),
                ICreateArguments a => new CreateCommand(a, _fileSystemLink, _fileWrap, _directoryWrap),
                IDeleteArguments a => new DeleteCommand(a, _fileWrap, _directoryWrap),
                IRelinkArguments a => new RelinkCommand(a, _hardLink, _junction, _symbolicLink, _fileSystemLink),
                IReparseArguments a => new ReparseCommand(a, _reparseOutput, _fileSystemLink),
                _ => throw new ArgumentException($"Unknown command argument type '{commandArguments.GetType()}'")
            };
        }
    }
}
