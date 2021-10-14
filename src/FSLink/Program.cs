using Autofac;
using Autofac.Extensions.DependencyInjection;
using FSLink.CommandLine;
using FSLinkCommand;
using FSLinkCommon;
using FSLinkLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

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
                var command = commandLineProcessor.Process(args);

                var application = serviceProvider.GetRequiredService<IApplicationHost>();
                var result = application.Run(command);

                return result;
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
                configure.AddConsole();
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
    }
}
