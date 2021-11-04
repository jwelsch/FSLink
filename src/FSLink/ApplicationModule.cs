using Autofac;
using FSLink.CommandLine;
using FSLinkCommand.Command;
using FSLinkCommand.Command.Reparse;
using FSLinkCommand.Command.Scan;

namespace FSLink
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Application
            builder.RegisterType<CommandLineProcessor>().As<ICommandLineProcessor>();
            builder.RegisterType<ApplicationHost>().As<IApplicationHost>();

            // Commands
            builder.RegisterType<CommandFactory>().As<ICommandFactory>();
            builder.RegisterType<LogScanOutput>().As<IScanOutput>();
            builder.RegisterType<LogReparseOutput>().As<IReparseOutput>();
        }
    }
}
