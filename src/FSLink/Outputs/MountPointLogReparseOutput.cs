using FSLinkCommand.Command.Reparse;
using FSLinkCommon.Util;
using FSLinkLib.ReparsePoints;
using Microsoft.Extensions.Logging;
using System;

#nullable enable

namespace FSLink.Outputs
{
    public interface IMountPointLogReparseOutput : IReparseOutput
    {
    }

    public class MountPointLogReparseOutput : IMountPointLogReparseOutput
    {
        private readonly ILogger _logger;

        public MountPointLogReparseOutput(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ReparseCommand>();
        }

        public void OnReparsePointData(IReparsePoint reparsePoint)
        {
            if (reparsePoint is not IMountPointReparsePoint mountPointReparsePoint)
            {
                throw new ArgumentException($"Cannot convert type '{reparsePoint.GetType()}' to type '{nameof(IMountPointReparsePoint)}'.", nameof(reparsePoint));
            }

            var message =
                $"File system entry\r\n" +
                $"  Path:                 {mountPointReparsePoint.Path}\r\n" +
                $"  IsDirectory:          {mountPointReparsePoint.IsDirectory}\r\n" +
                $"Reparse point\r\n" +
                $"  Tag:                  {mountPointReparsePoint.Tag.RawData.ToHexString()}\r\n" +
                $"    IsMicrosoft:        {mountPointReparsePoint.Tag.IsMicrosoft}\r\n" +
                $"    ReservedFlag0:      {mountPointReparsePoint.Tag.ReservedFlag0}\r\n" +
                $"    IsNameSurrogate:    {mountPointReparsePoint.Tag.IsNameSurrogate}\r\n" +
                $"    ReservedFlag1:      {mountPointReparsePoint.Tag.ReservedFlag1}\r\n" +
                $"    ReservedBits:       {mountPointReparsePoint.Tag.ReservedBits.ToHexString()}\r\n" +
                $"    TagValue:           {mountPointReparsePoint.Tag.TagValue.ToHexString()}\r\n" +
                $"    TagName:            {mountPointReparsePoint.TagName}\r\n" +
                $"  DataLength:           {mountPointReparsePoint.DataLength} (0x{mountPointReparsePoint.DataLength:X4}) bytes\r\n" +
                $"  Reserved:             {mountPointReparsePoint.Reserved.ToHexString()}\r\n" +
                $"Reparse data:\r\n" +
                $"  SubstituteNameOffset: {mountPointReparsePoint.SubstituteNameOffset} (0x{mountPointReparsePoint.SubstituteNameOffset:X4}) bytes\r\n" +
                $"  SubstituteNameLength: {mountPointReparsePoint.SubstituteNameLength} (0x{mountPointReparsePoint.SubstituteNameLength:X4}) bytes\r\n" +
                $"  PrintNameOffset:      {mountPointReparsePoint.PrintNameOffset} (0x{mountPointReparsePoint.PrintNameOffset:X4}) bytes\r\n" +
                $"  PrintNameLength:      {mountPointReparsePoint.PrintNameLength} (0x{mountPointReparsePoint.PrintNameLength:X4}) bytes\r\n" +
                $"Data buffer:\r\n" +
                $"{mountPointReparsePoint.Data.ToHexBlock(0, (int)mountPointReparsePoint.DataBufferLength)}";

            _logger.LogInformation(message);
        }
    }
}
