using Autofac.Extras.NSubstitute;
using FluentAssertions;
using FSLink.Outputs;
using FSLink.Tests.Fakes;
using FSLinkLib.ReparsePoints;
using System;
using Xunit;

#nullable enable

namespace FSLink.Tests.Outputs
{
    public class LogReparseOutputFactoryTests
    {
        [Fact]
        public void When_reparse_point_is_null_then_throw_argumentnullexception()
        {
            using var autoSub = new AutoSubstitute();

            var sut = autoSub.Resolve<LogReparseOutputFactory>();

            Action act = () => sut.Create(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void When_reparse_point_is_symboliclinkreparsepoint_then_symboliclinklogreparseoutput()
        {
            using var autoSub = new AutoSubstitute();

            var reparsePoint = autoSub.Resolve<ISymbolicLinkReparsePoint>();

            var sut = autoSub.Resolve<LogReparseOutputFactory>();

            var result = sut.Create(reparsePoint);

            result.Should().BeOfType(typeof(SymbolicLinkLogReparseOutput));
        }

        [Fact]
        public void When_reparse_point_is_mountpointreparsepoint_then_mountpointlogreparseoutput()
        {
            using var autoSub = new AutoSubstitute();

            var reparsePoint = autoSub.Resolve<IMountPointReparsePoint>();

            var sut = autoSub.Resolve<LogReparseOutputFactory>();

            var result = sut.Create(reparsePoint);

            result.Should().BeOfType(typeof(MountPointLogReparseOutput));
        }

        [Fact]
        public void When_reparse_point_is_datareparsepoint_then_datalogreparseoutput()
        {
            using var autoSub = new AutoSubstitute();

            var reparsePoint = autoSub.Resolve<IDataReparsePoint>();

            var sut = autoSub.Resolve<LogReparseOutputFactory>();

            var result = sut.Create(reparsePoint);

            result.Should().BeOfType(typeof(DataLogReparseOutput));
        }

        [Fact]
        public void When_reparse_point_is_unknown_then_throw_argumentexception()
        {
            using var autoSub = new AutoSubstitute();

            var sut = autoSub.Resolve<LogReparseOutputFactory>();

            Action act = () => sut.Create(autoSub.Resolve<IFakeReparsePoint>());

            act.Should().Throw<ArgumentException>();
        }
    }
}
