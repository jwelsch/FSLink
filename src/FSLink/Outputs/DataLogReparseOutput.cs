using FSLinkCommand.Command.Reparse;
using FSLinkCommon.Util;
using FSLinkLib.ReparsePoints;
using Microsoft.Extensions.Logging;
using System;

#nullable enable

namespace FSLink.Outputs
{
    public interface IDataLogReparseOutput : IReparseOutput
    {
    }

    public class DataLogReparseOutput : IDataLogReparseOutput
    {
        private readonly ILogger _logger;

        public DataLogReparseOutput(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ReparseCommand>();
        }

        public void OnReparsePointData(IReparsePoint reparsePoint)
        {
            if (reparsePoint is not IDataReparsePoint dataReparsePoint)
            {
                throw new ArgumentException($"Cannot convert type '{reparsePoint.GetType()}' to type '{nameof(IDataReparsePoint)}'.", nameof(reparsePoint));
            }

            var message =
                $"File system entry\r\n" +
                $"  Path:              {dataReparsePoint.Path}\r\n" +
                $"  IsDirectory:       {dataReparsePoint.IsDirectory}\r\n" +
                $"Reparse point\r\n" +
                $"  Tag:               {dataReparsePoint.Tag.RawData.ToHexString()}\r\n" +
                $"    IsMicrosoft:     {dataReparsePoint.Tag.IsMicrosoft}\r\n" +
                $"    ReservedFlag0:   {dataReparsePoint.Tag.ReservedFlag0}\r\n" +
                $"    IsNameSurrogate: {dataReparsePoint.Tag.IsNameSurrogate}\r\n" +
                $"    ReservedFlag1:   {dataReparsePoint.Tag.ReservedFlag1}\r\n" +
                $"    ReservedBits:    {dataReparsePoint.Tag.ReservedBits.ToHexString()}\r\n" +
                $"    TagValue:        {dataReparsePoint.Tag.TagValue.ToHexString()}\r\n" +
                $"    TagName:         {dataReparsePoint.TagName}\r\n" +
                $"  DataLength:        {dataReparsePoint.DataLength} (0x{dataReparsePoint.DataLength:X4}) bytes\r\n" +
                $"  Reserved:          {dataReparsePoint.Reserved.ToHexString()}\r\n" +
                $"Reparse data:\r\n" +
                $"{dataReparsePoint.Data.ToHexBlock(0, (int)dataReparsePoint.DataBufferLength)}";

            _logger.LogInformation(message);
        }
    }
}
