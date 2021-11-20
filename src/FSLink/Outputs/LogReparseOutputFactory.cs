using FSLinkCommand.Command.Reparse;
using FSLinkCommon.Wraps;
using FSLinkLib.ReparsePoints;
using System;

#nullable enable

namespace FSLink.Outputs
{
    public class LogReparseOutputFactory : ILogReparseOutputFactory
    {
        private readonly ILoggerFactoryWrap _loggerFactory;

        public LogReparseOutputFactory(ILoggerFactoryWrap loggerFactory)
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
