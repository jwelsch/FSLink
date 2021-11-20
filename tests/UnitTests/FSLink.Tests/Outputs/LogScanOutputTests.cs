using Autofac.Extras.NSubstitute;
using AutoFixture;
using FluentAssertions;
using FSLink.Outputs;
using FSLinkCommand.Command.Scan;
using FSLinkCommon.Wraps;
using FSLinkLib;
using NSubstitute;
using System;
using Xunit;

#nullable enable

namespace FSLink.Tests.Outputs
{
    public class LogScanOutputTests
    {
        private readonly static Fixture AutoFixture = new();

        [Fact]
        public void When_onfilesystementry_called_then_loginformation_called()
        {
            var path = AutoFixture.Create<string>();
            var linkType = AutoFixture.Create<FileSystemLinkType>();

            using var autoSub = new AutoSubstitute();

            var logger = autoSub.Resolve<ILoggerWrap<ScanCommand>>();

            var factory = autoSub.Resolve<ILoggerFactoryWrap>();
            factory.CreateLogger<ScanCommand>().Returns(logger);

            var sut = autoSub.Resolve<LogScanOutput>();

            var result = sut.OnFileSystemEntry(path, linkType);

            result.Should().BeTrue();
            logger.Received(1).LogInformation($"'{path}' has link type '{linkType}'");
        }

        [Fact]
        public void When_onfilesystemerror_called_then_logerror_called()
        {
            var path = AutoFixture.Create<string>();
            var error = AutoFixture.Create<Exception>();

            using var autoSub = new AutoSubstitute();

            var logger = autoSub.Resolve<ILoggerWrap<ScanCommand>>();

            var factory = autoSub.Resolve<ILoggerFactoryWrap>();
            factory.CreateLogger<ScanCommand>().Returns(logger);

            var sut = autoSub.Resolve<LogScanOutput>();

            var result = sut.OnFileSystemError(path, error);

            result.Should().BeTrue();
            logger.Received(1).LogError($"Error while getting link type for path '{path}': {error.Message}");
        }
    }
}
