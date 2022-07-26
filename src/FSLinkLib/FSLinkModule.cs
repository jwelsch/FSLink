using Autofac;
using FSLinkLib.PInvoke;

namespace FSLinkLib
{
    public class FSLinkModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NativeMethodCaller>().As<INativeMethodCaller>();
            builder.RegisterType<FileSystemLink>().As<IFileSystemLink>();
            builder.RegisterType<FileSystemPrivilege>().As<IFileSystemPrivilege>();
        }
    }
}
