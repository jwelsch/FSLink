using FSLinkCommand.Command.Scan;
using FSLinkCommon.Util;
using FSLinkCommon.Wraps;
using FSLinkLib;
using FSLinkLib.ReparsePoints;
using System;
using System.Collections.Generic;

#nullable enable

namespace FSLink.Outputs
{
    public class LogScanOutput : IScanOutput
    {
        private readonly IEnumerable<FileSystemLinkType> _fileSystemLinkTypeValues;
        private readonly IEnumerable<string> _reparseValues;

        private readonly ILoggerWrap _logger;
        private readonly IStringPaddingCalculator _stringPaddingCalculator;

        public LogScanOutput(ILoggerFactoryWrap loggerFactory, IStringPaddingCalculator stringPaddingCalculator)
        {
            _fileSystemLinkTypeValues = Enum.GetValues<FileSystemLinkType>();
            _reparseValues = new string[] { "Reparse", string.Empty };

            _logger = loggerFactory.CreateLogger<ScanCommand>();

            _stringPaddingCalculator = stringPaddingCalculator;
        }

        public bool OnFileSystemEntry(string path, FileSystemLinkType linkType, IReparsePoint? reparsePoint)
        {
            var linkTypeString = _stringPaddingCalculator.Calculate(linkType, _fileSystemLinkTypeValues);
            var reparseString = _stringPaddingCalculator.Calculate(reparsePoint == null ? string.Empty : "Reparse", _reparseValues);

            _logger.LogInformation($"{linkTypeString}\t{reparseString}\t{path}");

            return true;
        }

        public bool OnFileSystemError(string path, Exception? error)
        {
            _logger.LogError($"Error while getting link type for path '{path}': {error?.Message}");
            return true;
        }
    }
}
