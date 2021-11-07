using FSLinkCommand.Command.Reparse;
using FSLinkLib.ReparsePoints;
using Microsoft.Extensions.Logging;
using System;

#nullable enable

namespace FSLink.Outputs
{
    public class LogReparseOutputFactory : ILogReparseOutputFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public LogReparseOutputFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public IReparseOutput Create(IReparsePoint? reparsePoint)
        {
            if (reparsePoint == null)
            {
                throw new ArgumentNullException(nameof(reparsePoint));
            }

            if (reparsePoint is ISymbolicLinkReparsePoint)
            {
                return new SymbolicLinkLogReparseOutput(_loggerFactory);
            }
            else if (reparsePoint is IMountPointReparsePoint)
            {
                return new MountPointLogReparseOutput(_loggerFactory);
            }
            else if (reparsePoint is IDataReparsePoint)
            {
                return new DataLogReparseOutput(_loggerFactory);
            }

            throw new ArgumentException($"The type '${reparsePoint?.GetType()}' is unexpected.", nameof(reparsePoint));
        }
    }
}
