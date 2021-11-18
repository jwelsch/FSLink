using FSLinkCommand.Command.Scan;
using FSLinkLib;
using Microsoft.Extensions.Logging;
using System;

namespace FSLink.Outputs
{
    public class LogScanOutput : IScanOutput
    {
        private readonly ILogger _logger;

        public LogScanOutput(ILoggerFactory loggerFactory)
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
