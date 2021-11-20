using FSLinkCommand.Command.Scan;
using FSLinkCommon.Wraps;
using FSLinkLib;
using System;

namespace FSLink.Outputs
{
    public class LogScanOutput : IScanOutput
    {
        private readonly ILoggerWrap _logger;

        public LogScanOutput(ILoggerFactoryWrap loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ScanCommand>();
        }

        public bool OnFileSystemEntry(string path, FileSystemLinkType linkType)
        {
            _logger.LogInformation($"'{path}' has link type '{linkType}'");
            return true;
        }

        public bool OnFileSystemError(string path, Exception error)
        {
            _logger.LogError($"Error while getting link type for path '{path}': {error.Message}");
            return true;
        }
    }
}
