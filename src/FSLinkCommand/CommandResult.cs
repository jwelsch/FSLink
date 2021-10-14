using System;

#nullable enable

namespace FSLinkCommand
{
    public interface ICommandResult
    {
        string CommandName { get; }

        bool Success { get; }

        Exception? Error { get; }

        object? Data { get; }
    }

    public abstract class CommandResult : ICommandResult
    {
        public string CommandName { get; }

        public bool Success { get; }

        public Exception? Error { get; }

        public object? Data { get; }

        protected CommandResult(bool success, string commandName, object? data)
            : this(success, commandName, data, null)
        {
        }

        protected CommandResult(bool success, string commandName, Exception? error)
            : this(success, commandName, null, error)
        {
        }

        private CommandResult(bool success, string commandName, object? data, Exception? error)
        {
            Success = success;
            CommandName = commandName;
            Data = data;
            Error = error;
        }
    }

    public class SuccessCommandResult : CommandResult
    {
        public SuccessCommandResult(string commandName, object? data = null)
            : base(true, commandName, data)
        {
        }
    }

    public class ErrorCommandResult : CommandResult
    {
        public ErrorCommandResult(string commandName, Exception? error)
            : base(false, commandName, error)
        {
        }
    }
}
