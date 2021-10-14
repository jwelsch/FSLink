using FSLinkCommand.Reparse;
using FSLinkLib;
using Microsoft.Extensions.Logging;

namespace FSLink
{
    public class LogReparseOutput : IReparseOutput
    {
        private readonly ILogger _logger;

        public LogReparseOutput(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ReparseCommand>();
        }

        public void OnReparsePointData(IReparsePoint reparsePoint)
        {
            var message =
@"File system entry
  Path:            {0}
  IsDirectory:     {1}
Reparse point data
  IsMicrosoft:     {2}
  ReservedFlag0:   {3}
  IsNameSurrogate: {4}
  ReservedFlag1:   {5}
  ReservedBits:    0x{6:X4}
  TagValue:        0x{7:X4}
  TagName:         {8}";

            _logger.LogInformation(string.Format(message,
                                                 reparsePoint.Path,
                                                 reparsePoint.IsDirectory,
                                                 reparsePoint.Tag.IsMicrosoft,
                                                 reparsePoint.Tag.ReservedFlag0,
                                                 reparsePoint.Tag.IsNameSurrogate,
                                                 reparsePoint.Tag.ReservedFlag1,
                                                 reparsePoint.Tag.ReservedBits,
                                                 reparsePoint.Tag.TagValue,
                                                 reparsePoint.TagName
            ));
        }
    }
}
