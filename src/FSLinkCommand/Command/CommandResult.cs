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
            : base(false, commandName, new CommandError(message, exception))
        {
        }

        public ErrorCommandResult(string commandName, Exception exception)
            : base(false, commandName, new CommandError(exception))
        {
        }
    }
}
