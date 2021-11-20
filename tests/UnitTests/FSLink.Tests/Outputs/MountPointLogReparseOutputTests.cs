using Autofac.Extras.NSubstitute;
using AutoFixture;
using FluentAssertions;
using FSLink.Outputs;
using FSLink.Tests.Fakes;
using FSLinkCommand.Command.Reparse;
using FSLinkCommon.Wraps;
using FSLinkLib.ReparsePoints;
using NSubstitute;
using System;
using System.Linq;
using Xunit;

#nullable enable

namespace FSLink.Tests.Outputs
{
    public class MountPointLogReparseOutputTests
    {
        private readonly static Fixture AutoFixture = new();

        [Fact]
        public void When_reparsepoint_is_not_mountpointreparsepoint_then_throw_argumentexception()
        {
            using var autoSub = new AutoSubstitute();

            var data = autoSub.Resolve<IFakeReparsePoint>();

            var sut = autoSub.Resolve<MountPointLogReparseOutput>();

            Action act = () => sut.OnReparsePointData(data);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void When_reparsepoint_is_mountpointreparsepoint_then_call_loginformation()
        {
            using var autoSub = new AutoSubstitute();

            var logger = autoSub.Resolve<ILoggerWrap<ReparseCommand>>();

            var factory = autoSub.Resolve<ILoggerFactoryWrap>();
            factory.CreateLogger<ReparseCommand>().Returns(logger);

            var sut = autoSub.Resolve<MountPointLogReparseOutput>();

            var tag = autoSub.Resolve<IReparseTag>();
            tag.RawData.Returns(AutoFixture.Create<uint>());
            tag.IsMicrosoft.Returns(AutoFixture.Create<bool>());
            tag.ReservedFlag0.Returns(AutoFixture.Create<bool>());
            tag.IsNameSurrogate.Returns(AutoFixture.Create<bool>());
            tag.ReservedFlag1.Returns(AutoFixture.Create<bool>());
            tag.ReservedBits.Returns(AutoFixture.Create<ushort>());
            tag.TagValue.Returns(AutoFixture.Create<ushort>());
            tag.IsMicrosoft.Returns(AutoFixture.Create<bool>());

            var data = autoSub.Resolve<IMountPointReparsePoint>();
            data.Path.Returns(AutoFixture.Create<string>());
            data.IsDirectory.Returns(AutoFixture.Create<bool>());
            data.TagName.Returns(AutoFixture.Create<string>());
            data.DataLength.Returns(AutoFixture.Create<uint>());
            data.Reserved.Returns(AutoFixture.Create<uint>());
            data.SubstituteNameOffset.Returns(AutoFixture.Create<ushort>());
            data.SubstituteNameLength.Returns(AutoFixture.Create<ushort>());
            data.PrintNameOffset.Returns(AutoFixture.Create<ushort>());
            data.PrintNameLength.Returns(AutoFixture.Create<ushort>());
            data.Data.Returns(AutoFixture.CreateMany<byte>(7).Select(i => i).ToArray());

            sut.OnReparsePointData(data);

            logger.Received(1).LogInformation(Arg.Any<string?>(), Arg.Any<object?[]>());
        }
    }
}
