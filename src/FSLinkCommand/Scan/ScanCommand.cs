using FSLinkLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FSLinkCommand.Scan
{
    public interface IScanArguments : ICommandArguments
    {
        string Path { get; }

        bool Recurse { get; }
    }

    public class ScanCommand : ICommandBase
    {
        private readonly IScanArguments _commandArguments;
        private readonly IFileSystemLink _fileSystemLink;
        private readonly IFileSystemScanner _fileSystemScanner;
        private readonly IScanOutput _scanOutput;

        public string Name => "Scan";

        public ScanCommand(IScanArguments commandArguments, IFileSystemLink fileSystemLink, IFileSystemScanner fileSystemScanner, IScanOutput scanOutput)
        {
            _commandArguments = commandArguments;
            _fileSystemLink = fileSystemLink;
            _fileSystemScanner = fileSystemScanner;
            _scanOutput = scanOutput;
        }

        public async Task<ICommandResult> Run()
        {
            return await Task.Run<ICommandResult>(() =>
            {
                var errors = new List<Exception>();

                _fileSystemScanner.ScanPath(_commandArguments.Path, "*", _commandArguments.Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly, path =>
                {
                    try
                    {
                        var linkType = _fileSystemLink.GetLinkType(path);
                        _scanOutput.OnFileSystemEntry(path, linkType);
                    }
                    catch (Exception ex)
                    {
                        _scanOutput.OnFileSystemError(path, ex);
                    }
                    return true;
                });

                return new SuccessCommandResult(Name, errors.ToArray());
            });
        }
    }
}
