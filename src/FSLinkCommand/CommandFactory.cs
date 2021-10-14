using FSLinkCommand.Create;
using FSLinkCommand.Delete;
using FSLinkCommand.Relink;
using FSLinkCommand.Reparse;
using FSLinkCommand.Scan;
using FSLinkCommon.Wraps;
using FSLinkLib;
using System;

namespace FSLinkCommand
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

        public CommandFactory(IFileSystemLink fileSystemLink, IFileSystemScanner fileSystemScanner, IFileWrap fileWrap, IDirectoryWrap directoryWrap, IScanOutput scanOutput, IReparseOutput reparseOutput)
        {
            _fileSystemLink = fileSystemLink;
            _fileSystemScanner = fileSystemScanner;
            _fileWrap = fileWrap;
            _directoryWrap = directoryWrap;
            _scanOutput = scanOutput;
            _reparseOutput = reparseOutput;
        }

        public ICommandBase Create(ICommandArguments commandArguments)
        {
            return commandArguments switch
            {
                IScanArguments a => new ScanCommand(a, _fileSystemLink, _fileSystemScanner, _scanOutput),
                ICreateArguments a => new CreateCommand(a, _fileSystemLink, _fileWrap, _directoryWrap),
                IDeleteArguments a => new DeleteCommand(a, _fileWrap, _directoryWrap),
                IRelinkArguments a => new RelinkCommand(a, _fileSystemLink, _fileWrap, _directoryWrap),
                IReparseArguments a => new ReparseCommand(a, _reparseOutput, _fileSystemLink),
                _ => throw new ArgumentException($"Unknown command argument type '{commandArguments.GetType()}'")
            };
        }
    }
}
