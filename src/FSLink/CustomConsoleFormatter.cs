using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using System.IO;

namespace FSLink
{
    public class CustomConsoleOptions : ConsoleFormatterOptions
    {
    }

    public class CustomConsoleFormatter : ConsoleFormatter
    {
        public CustomConsoleFormatter()
            : base("CustomConsoleFormatter")
        {
        }

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
        {
            var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);

            if (message == null)
            {
                return;
            }

            textWriter.WriteLine(message);
        }
    }
}
