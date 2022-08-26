using Autofac;
using Autofac.Extensions.DependencyInjection;
using CommandLine;
using FSLink.CommandLine;
using FSLinkCommand.Command;
using FSLinkCommon;
using FSLinkLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace FSLink
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                using var serviceProvider = RegisterServices();

                var commandLineProcessor = serviceProvider.GetRequiredService<ICommandLineProcessor>();
                var commandArguments = commandLineProcessor.Process(args);

                var commandFactory = serviceProvider.GetRequiredService<ICommandFactory>();
                var command = commandFactory.Create(commandArguments);

                var application = serviceProvider.GetRequiredService<IApplicationHost>();
                var result = application.Run(command, commandArguments);

                return result;
            }
            catch (CommandLineException ex)
            {
                var joined = ex.Errors.Aggregate(string.Empty, (joined, next) => joined += $"{(joined.Length == 0 ? string.Empty : Environment.NewLine)}{ErrorToString(next)}");

                var message = $"Command line error:{Environment.NewLine}{joined}";
                System.Diagnostics.Trace.WriteLine(message);
                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                var message = $"Error - caught exception: {ex}";
                System.Diagnostics.Trace.WriteLine(message);
                Console.WriteLine(message);
            }

            return int.MinValue;
        }

        private static AutofacServiceProvider RegisterServices()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var builder = new ContainerBuilder();
            builder.Populate(services);

            RegisterModules(builder);

            return new AutofacServiceProvider(builder.Build());
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure =>
            {
                configure.AddConsole(o => o.FormatterName = "CustomConsoleFormatter")
                         .AddConsoleFormatter<CustomConsoleFormatter, CustomConsoleOptions>();
                configure.AddDebug();
            });
        }

        private static void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterModule<CommonModule>();
            builder.RegisterModule<FSLinkModule>();
            builder.RegisterModule<CommandModule>();
            builder.RegisterModule<ApplicationModule>();
        }

        private static string ErrorToString(Error error)
        {
            if (error is NamedError namedError)
            {
                return $"{error.Tag}: \"{namedError.NameInfo.NameText}\"";
            }

            return error.Tag.ToString();
        }
    }
}
