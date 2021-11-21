using FSLinkCommand.Command;
using FSLinkCommon.Wraps;

namespace FSLink
{
    public interface IApplicationHost
    {
        int Run(ICommandBase command, ICommandArguments arguments);
    }

    public class ApplicationHost : IApplicationHost
    {
        private readonly ILoggerWrap _logger;
        private readonly ICommandRunner _commandRunner;

        public ApplicationHost(ILoggerFactoryWrap loggerFactory, ICommandRunner commandRunner)
        {
            _logger = loggerFactory.CreateLogger<ApplicationHost>();
            _commandRunner = commandRunner;
        }

        public int Run(ICommandBase command, ICommandArguments arguments)
        {
            var result = _commandRunner.Execute(command, arguments);

            if (result.Success)
            {
                _logger.LogInformation($"The {result.CommandName} command has completed successfully.");
                return 0;
            }

            _logger.LogError($"The {result.CommandName} command has completed with an error: {result.Error}");
            return result.Error?.Code ?? int.MinValue;
        }
    }
}
