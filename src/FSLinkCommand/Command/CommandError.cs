using System;

#nullable enable

namespace FSLinkCommand.Command
{
    public class CommandError
    {
        private string? _asString;

        public string? Message { get; }

        public int? Code { get; }

        public Exception? Exception { get; }

        protected CommandError(string? message, int? code, Exception? exception)
        {
            Message = message;
            Code = code;
            Exception = exception;
        }

        public CommandError(string message)
            : this(message, null, null)
        {
        }

        public CommandError(string message, int code)
            : this(message, code, null)
        {
        }

        public CommandError(string message, Exception exception)
            : this(message, null, exception)
        {
        }

        public CommandError(int code)
            : this(null, code, null)
        {
        }

        public CommandError(int code, Exception exception)
            : this(null, code, exception)
        {
        }

        public CommandError(Exception exception)
            : this(null, null, exception)
        {
        }

        public override string ToString()
        {
            if (_asString == null)
            {
                var builder = new System.Text.StringBuilder();

                if (!string.IsNullOrEmpty(Message))
                {
                    builder.Append(Message);
                }

                if (Code != null)
                {
                    builder.Append(builder.Length > 0 ? $" ({Code})" : $"{Code}");
                }

                if (Exception != null)
                {
                    builder.Append(builder.Length > 0 ? $" - {Exception}" : $"{Exception}");
                }

                _asString = builder.ToString();
            }

            return _asString;
        }
    }
}
