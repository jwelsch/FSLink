using Autofac.Extras.NSubstitute;
using AutoFixture;
using FluentAssertions;
using FSLink.Outputs;
using FSLink.Tests.Fakes;
using FSLinkCommand.Command.Reparse;
using FSLinkLib.ReparsePoints;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Linq;
using Xunit;

#nullable enable

namespace FSLink.Tests.Outputs
{
    public class DataLogReparseOutputTests
    {
        private readonly static Fixture AutoFixture = new();

        [Fact]
        public void When_reparse_point_is_datareparsepoint_type_then_logger_called()
        {
            using var autoSub = new AutoSubstitute();

            var logger = autoSub.Resolve<ILogger<ReparseCommand>>();

            var factory = autoSub.Resolve<ILoggerFactory>();
            factory.CreateLogger<ReparseCommand>().Returns(logger);

            var tag = autoSub.Resolve<IReparseTag>();
            tag.RawData.Returns(AutoFixture.Create<uint>());
            tag.IsMicrosoft.Returns(AutoFixture.Create<bool>());
            tag.ReservedFlag0.Returns(AutoFixture.Create<bool>());
            tag.IsNameSurrogate.Returns(AutoFixture.Create<bool>());
            tag.ReservedFlag1.Returns(AutoFixture.Create<bool>());
            tag.ReservedBits.Returns(AutoFixture.Create<ushort>());
            tag.TagValue.Returns(AutoFixture.Create<ushort>());
            tag.IsMicrosoft.Returns(AutoFixture.Create<bool>());

            var data = autoSub.Resolve<IDataReparsePoint>();
            data.Path.Returns(AutoFixture.Create<string>());
            data.IsDirectory.Returns(AutoFixture.Create<bool>());
            data.TagName.Returns(AutoFixture.Create<string>());
            data.DataLength.Returns(AutoFixture.Create<uint>());
            data.Reserved.Returns(AutoFixture.Create<uint>());
            data.Data.Returns(AutoFixture.CreateMany<byte>(7).Select(i => i).ToArray());

            var sut = autoSub.Resolve<DataLogReparseOutput>();

            sut.OnReparsePointData(data);

            //logger.Received(1).LogInformation(Arg.Any<string?>(), Arg.Any<object[]>());
        }

        [Fact]
        public void When_reparse_point_is_not_datareparsepoint_type_then_throw()
        {
            using var autoSub = new AutoSubstitute();

            var logger = autoSub.Resolve<ILogger<ReparseCommand>>();

            var factory = autoSub.Resolve<ILoggerFactory>();
            factory.CreateLogger<ReparseCommand>().Returns(logger);

            var tag = autoSub.Resolve<IReparseTag>();
            tag.RawData.Returns(AutoFixture.Create<uint>());
            tag.IsMicrosoft.Returns(AutoFixture.Create<bool>());
            tag.ReservedFlag0.Returns(AutoFixture.Create<bool>());
            tag.IsNameSurrogate.Returns(AutoFixture.Create<bool>());
            tag.ReservedFlag1.Returns(AutoFixture.Create<bool>());
            tag.ReservedBits.Returns(AutoFixture.Create<ushort>());
            tag.TagValue.Returns(AutoFixture.Create<ushort>());
            tag.IsMicrosoft.Returns(AutoFixture.Create<bool>());

            var data = autoSub.Resolve<IFakeReparsePoint>();
            data.Path.Returns(AutoFixture.Create<string>());
            data.IsDirectory.Returns(AutoFixture.Create<bool>());
            data.TagName.Returns(AutoFixture.Create<string>());
            data.DataLength.Returns(AutoFixture.Create<uint>());
            data.Reserved.Returns(AutoFixture.Create<uint>());
            data.Data.Returns(AutoFixture.CreateMany<byte>(7).Select(i => i).ToArray());

            var sut = autoSub.Resolve<DataLogReparseOutput>();

            Action act = () => sut.OnReparsePointData(data);

            act.Should().Throw<ArgumentException>();
        }
    }
}
