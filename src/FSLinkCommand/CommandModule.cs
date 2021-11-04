using Autofac;
using FSLinkCommand.Command.Scan;
using FSLinkCommand.FileSystem;

namespace FSLinkCommand.Command
{
    public class CommandModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Scanning
            builder.RegisterType<FileSystemScanner>().As<IFileSystemScanner>();

            // File system
            builder.RegisterType<HardLink>().As<IHardLink>();
            builder.RegisterType<Junction>().As<IJunction>();
            builder.RegisterType<SymbolicLink>().As<ISymbolicLink>();
        }
    }
}
