using FSLinkLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FSLinkCommand.Command.Scan
{
    public class ScanCommand : CommandBase<IScanArguments>
    {
        private readonly IFileSystemLink _fileSystemLink;
        private readonly IFileSystemScanner _fileSystemScanner;
        private readonly IScanOutput _scanOutput;

        public ScanCommand(IFileSystemLink fileSystemLink, IFileSystemScanner fileSystemScanner, IScanOutput scanOutput)
            : base("Scan")
        {
            _fileSystemLink = fileSystemLink;
            _fileSystemScanner = fileSystemScanner;
            _scanOutput = scanOutput;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected override async Task<ICommandResult> DoRun(IScanArguments arguments)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var errors = new List<Exception>();

            _fileSystemScanner.ScanPath(arguments.Path, "*", arguments.Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly, path =>
            {
                try
                {
                    var linkType = _fileSystemLink.GetLinkType(path);
                    return _scanOutput.OnFileSystemEntry(path, linkType);
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                    return _scanOutput.OnFileSystemError(path, ex);
                }
            });

            return new SuccessCommandResult(Name, errors.ToArray());
        }
    }
}
