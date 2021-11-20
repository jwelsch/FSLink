using Autofac.Extras.NSubstitute;
using AutoFixture;
using FluentAssertions;
using FSLink.CommandLine;
using System;
using Xunit;

#nullable enable

namespace FSLink.Tests.CommandLine
{
    public class CommandLineProcessorTests
    {
        private readonly static Fixture AutoFixture = new();

        [Fact]
        public void When_empty_arguments_given_then_return_null()
        {
            using var autoSub = new AutoSubstitute();

            var args = Array.Empty<string>();

            var sut = autoSub.Resolve<CommandLineProcessor>();

            var result = sut.Process(args);

            result.Should().BeNull();
        }

        [Fact]
        public void When_create_arguments_given_then_return_createarguments()
        {
            using var autoSub = new AutoSubstitute();

            var linkPath = AutoFixture.Create<string>();
            var targetPath = AutoFixture.Create<string>();

            var args = new[]
            {
                "create",
                "--link-type",
                "HardLink",
                "--link-path",
                linkPath,
                "--target-path",
                targetPath
            };

            var sut = autoSub.Resolve<CommandLineProcessor>();

            var result = sut.Process(args);

            result.Should().BeOfType(typeof(CreateArguments));
        }

        [Fact]
        public void When_delete_arguments_given_then_return_deletearguments()
        {
            using var autoSub = new AutoSubstitute();

            var path = AutoFixture.Create<string>();

            var args = new[]
            {
                "delete",
                "--path",
                path
            };

            var sut = autoSub.Resolve<CommandLineProcessor>();

            var result = sut.Process(args);

            result.Should().BeOfType(typeof(DeleteArguments));
        }

        [Fact]
        public void When_relink_arguments_given_then_return_relinkarguments()
        {
            using var autoSub = new AutoSubstitute();

            var linkPath = AutoFixture.Create<string>();
            var targetPath = AutoFixture.Create<string>();

            var args = new[]
            {
                "relink",
                "--link-path",
                linkPath,
                "--new-target-path",
                targetPath
            };

            var sut = autoSub.Resolve<CommandLineProcessor>();

            var result = sut.Process(args);

            result.Should().BeOfType(typeof(RelinkArguments));
        }

        [Fact]
        public void When_reparse_arguments_given_then_return_reparsearguments()
        {
            using var autoSub = new AutoSubstitute();

            var path = AutoFixture.Create<string>();

            var args = new[]
            {
                "reparse",
                "--path",
                path
            };

            var sut = autoSub.Resolve<CommandLineProcessor>();

            var result = sut.Process(args);

            result.Should().BeOfType(typeof(ReparseArguments));
        }

        [Fact]
        public void When_scan_arguments_given_then_return_scanarguments()
        {
            using var autoSub = new AutoSubstitute();

            var path = AutoFixture.Create<string>();
            var recurse = AutoFixture.Create<bool>();

            var args = new[]
            {
                "scan",
                "--path",
                path,
                "--recurse",
                recurse.ToString()
            };

            var sut = autoSub.Resolve<CommandLineProcessor>();

            var result = sut.Process(args);

            result.Should().BeOfType(typeof(ScanArguments));
        }
    }
}
