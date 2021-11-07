using FSLinkCommand.Command.Reparse;
using FSLinkCommon.Util;
using FSLinkLib.ReparsePoints;
using Microsoft.Extensions.Logging;
using System;

#nullable enable

namespace FSLink.Outputs
{
    public interface ISymbolicLinkLogReparseOutput : IReparseOutput
    {
    }

    public class SymbolicLinkLogReparseOutput : ISymbolicLinkLogReparseOutput
    {
        private readonly ILogger _logger;

        public SymbolicLinkLogReparseOutput(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ReparseCommand>();
        }

        public void OnReparsePointData(IReparsePoint reparsePoint)
        {
            if (reparsePoint is not ISymbolicLinkReparsePoint symbolicLinkReparsePoint)
            {
                throw new ArgumentException($"Cannot convert type '{reparsePoint.GetType()}' to type '{nameof(ISymbolicLinkReparsePoint)}'.", nameof(reparsePoint));
            }

            var message =
                $"File system entry\r\n" +
                $"  Path:                 {symbolicLinkReparsePoint.Path}\r\n" +
                $"  IsDirectory:          {symbolicLinkReparsePoint.IsDirectory}\r\n" +
                $"Reparse point\r\n" +
                $"  Tag:                  {symbolicLinkReparsePoint.Tag.RawData.ToHexString()}\r\n" +
                $"    IsMicrosoft:        {symbolicLinkReparsePoint.Tag.IsMicrosoft}\r\n" +
                $"    ReservedFlag0:      {symbolicLinkReparsePoint.Tag.ReservedFlag0}\r\n" +
                $"    IsNameSurrogate:    {symbolicLinkReparsePoint.Tag.IsNameSurrogate}\r\n" +
                $"    ReservedFlag1:      {symbolicLinkReparsePoint.Tag.ReservedFlag1}\r\n" +
                $"    ReservedBits:       {symbolicLinkReparsePoint.Tag.ReservedBits.ToHexString()}\r\n" +
                $"    TagValue:           {symbolicLinkReparsePoint.Tag.TagValue.ToHexString()}\r\n" +
                $"    TagName:            {symbolicLinkReparsePoint.TagName}\r\n" +
                $"  DataLength:           {symbolicLinkReparsePoint.DataLength} (0x{symbolicLinkReparsePoint.DataLength:X4}) bytes\r\n" +
                $"  Reserved:             {symbolicLinkReparsePoint.Reserved.ToHexString()}\r\n" +
                $"Reparse data:\r\n" +
                $"  SubstituteNameOffset: {symbolicLinkReparsePoint.SubstituteNameOffset} (0x{symbolicLinkReparsePoint.SubstituteNameOffset:X4}) bytes\r\n" +
                $"  SubstituteNameLength: {symbolicLinkReparsePoint.SubstituteNameLength} (0x{symbolicLinkReparsePoint.SubstituteNameLength:X4}) bytes\r\n" +
                $"  PrintNameOffset:      {symbolicLinkReparsePoint.PrintNameOffset} (0x{symbolicLinkReparsePoint.PrintNameOffset:X4}) bytes\r\n" +
                $"  PrintNameLength:      {symbolicLinkReparsePoint.PrintNameLength} (0x{symbolicLinkReparsePoint.PrintNameLength:X4}) bytes\r\n" +
                $"  Flags:                0x{symbolicLinkReparsePoint.Flags:X8}\r\n" +
                $"Data buffer:\r\n" +
                $"{symbolicLinkReparsePoint.Data.ToHexBlock(0, (int)symbolicLinkReparsePoint.DataBufferLength)}";

            _logger.LogInformation(message);
        }
    }
}
