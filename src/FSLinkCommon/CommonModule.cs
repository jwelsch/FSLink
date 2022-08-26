using Autofac;
using FSLinkCommon.Util;
using FSLinkCommon.Wraps;

namespace FSLinkCommon
{
    public class CommonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Wraps
            builder.RegisterType<DirectoryWrap>().As<IDirectoryWrap>();
            builder.RegisterType<FileWrap>().As<IFileWrap>();
            builder.RegisterType<LoggerWrap>().As<ILoggerWrap>();
            builder.RegisterType<LoggerFactoryWrap>().As<ILoggerFactoryWrap>();

            builder.RegisterType<StringPaddingCalculator>().As<IStringPaddingCalculator>();
            builder.RegisterType<EnumValueStringLengthCalculator>().As<IEnumValueStringLengthCalculator>();
        }
    }
}
