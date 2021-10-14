using Autofac;
using FSLinkCommand.Scan;

namespace FSLinkCommand
{
    public class CommandModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Scanning
            builder.RegisterType<FileSystemScanner>().As<IFileSystemScanner>();
        }
    }
}
