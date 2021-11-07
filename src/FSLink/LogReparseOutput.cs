using FSLinkCommand.Command.Reparse;
using FSLinkCommon.Util;
using FSLinkLib.ReparsePoints;
using Microsoft.Extensions.Logging;

#nullable enable

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
                $"File system entry\r\n" +
                $"  Path:              {reparsePoint.Path}\r\n" +
                $"  IsDirectory:       {reparsePoint.IsDirectory}\r\n" +
                $"Reparse point\r\n" +
                $"  Tag:               {reparsePoint.Tag.RawData.ToHexString()}\r\n" +
                $"    IsMicrosoft:     {reparsePoint.Tag.IsMicrosoft}\r\n" +
                $"    ReservedFlag0:   {reparsePoint.Tag.ReservedFlag0}\r\n" +
                $"    IsNameSurrogate: {reparsePoint.Tag.IsNameSurrogate}\r\n" +
                $"    ReservedFlag1:   {reparsePoint.Tag.ReservedFlag1}\r\n" +
                $"    ReservedBits:    {reparsePoint.Tag.ReservedBits.ToHexString()}\r\n" +
                $"    TagValue:        {reparsePoint.Tag.TagValue.ToHexString()}\r\n" +
                $"    TagName:         {reparsePoint.TagName}\r\n" +
                $"  DataLength:        {reparsePoint.DataLength} (0x{reparsePoint.DataLength:X4}) bytes\r\n" +
                $"  Reserved:          {reparsePoint.Reserved.ToHexString()}\r\n" +
                $"\r\n" +
                $"Reparse data:\r\n" +
                $"{reparsePoint.Data.ToHexBlock(0, (int)reparsePoint.DataBufferLength)}";

            _logger.LogInformation(message);
        }
    }
}
