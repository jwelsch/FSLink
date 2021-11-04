using System;

#nullable enable

namespace FSLinkCommand.Command
{
    public interface ICommandResult
    {
        string CommandName { get; }

        bool Success { get; }

        CommandError? Error { get; }

        object? Data { get; }
    }

    public class CommandError
    {
        public string? Message { get; }

        public int? Code { get; }

        public Exception? Exception { get; }

        public CommandError(string? message, int? code, Exception? exception)
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

        public CommandError(Exception exception)
            : this(null, null, exception)
        {
        }
    }

    public abstract class CommandResult : ICommandResult
    {
        public string CommandName { get; }

        public bool Success { get; }

        public CommandError? Error { get; }

        public object? Data { get; }

        protected CommandResult(bool success, string commandName, object? data)
            : this(success, commandName, data, null)
        {
        }

        protected CommandResult(bool success, string commandName, CommandError? error)
            : this(success, commandName, null, error)
        {
        }

        private CommandResult(bool success, string commandName, object? data, CommandError? error)
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
        public ErrorCommandResult(string commandName, CommandError error)
            : base(false, commandName, error)
        {
        }

        public ErrorCommandResult(string commandName, string message)
            : base(false, commandName, new CommandError(message))
        {
        }

        public ErrorCommandResult(string commandName, string message, int code)
            : base(false, commandName, new CommandError(message, code))
        {
        }

        public ErrorCommandResult(string commandName, string message, Exception exception)
            : base(false, commandName, new CommandError(message, null, exception))
        {
        }

        public ErrorCommandResult(string commandName, Exception exception)
            : base(false, commandName, new CommandError(null, null, exception))
        {
        }
    }
}
