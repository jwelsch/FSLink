using FSLinkCommand;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FSLink
{
    public interface IApplicationHost
    {
        int Run(ICommandBase command);
    }

    public class ApplicationHost : IApplicationHost
    {
        private readonly ILogger _logger;

        public ApplicationHost(ILogger<ApplicationHost> logger)
        {
            _logger = logger;
        }

        public int Run(ICommandBase command)
        {
            var task = Task.Run(() => command.Run());
            var awaiter = task.GetAwaiter();
            var result = awaiter.GetResult();

            if (result.Success)
            {
                _logger.LogInformation($"The {result.CommandName} command has completed successfully.");
                return 0;
            }

            _logger.LogError($"The {result.CommandName} command has completed with an error: {result.Error}");
            return -1;
        }
    }
}
