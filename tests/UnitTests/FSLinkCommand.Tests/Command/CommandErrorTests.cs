using Autofac.Extras.NSubstitute;
using AutoFixture;
using FluentAssertions;
using FSLinkCommand.Command;
using System;
using Xunit;

namespace FSLinkCommand.Tests.Command
{
    //
    // Note: Autofac does not work with the CommandError constructors.
    // It gets confused and calls the wrong one even though the correct
    // parameter(s) are passed to the Resolve<T>() method.
    //

    public class CommandErrorTests
    {
        private readonly static Fixture AutoFixture = new();

        [Fact]
        public void When_string_ctor_called_then_properties_and_tostring_are_as_expected()
        {
            using var autoSub = new AutoSubstitute();

            var message = AutoFixture.Create<string>();

            var sut = new CommandError(message);

            sut.Message.Should().Be(message);
            sut.Code.Should().BeNull();
            sut.Exception.Should().BeNull();
            sut.ToString().Should().Be($"{message}");
        }

        [Fact]
        public void When_string_int_ctor_called_then_properties_and_tostring_are_as_expected()
        {
            using var autoSub = new AutoSubstitute();

            var message = AutoFixture.Create<string>();
            var code = AutoFixture.Create<int>();

            var sut = new CommandError(message, code);

            sut.Message.Should().Be(message);
            sut.Code.Should().Be(code);
            sut.Exception.Should().BeNull();
            sut.ToString().Should().Be($"{message} ({code})");
        }

        [Fact]
        public void When_string_exception_ctor_called_then_properties_and_tostring_are_as_expected()
        {
            using var autoSub = new AutoSubstitute();

            var message = AutoFixture.Create<string>();
            var exception = AutoFixture.Create<Exception>();

            var sut = new CommandError(message, exception);

            sut.Message.Should().Be(message);
            sut.Code.Should().BeNull();
            sut.Exception.Should().Be(exception);
            sut.ToString().Should().Be($"{message} - {exception}");
        }

        [Fact]
        public void When_int_ctor_called_then_properties_and_tostring_are_as_expected()
        {
            using var autoSub = new AutoSubstitute();

            var code = AutoFixture.Create<int>();

            var sut = new CommandError(code);

            sut.Message.Should().BeNull();
            sut.Code.Should().Be(code);
            sut.Exception.Should().BeNull();
            sut.ToString().Should().Be($"{code}");
        }

        [Fact]
        public void When_int_exception_ctor_called_then_properties_and_tostring_are_as_expected()
        {
            using var autoSub = new AutoSubstitute();

            var code = AutoFixture.Create<int>();
            var exception = AutoFixture.Create<Exception>();

            var sut = new CommandError(code, exception);

            sut.Message.Should().BeNull();
            sut.Code.Should().Be(code);
            sut.Exception.Should().Be(exception);
            sut.ToString().Should().Be($"{code} - {exception}");
        }

        [Fact]
        public void When_exception_ctor_called_then_properties_and_tostring_are_as_expected()
        {
            using var autoSub = new AutoSubstitute();

            var exception = AutoFixture.Create<Exception>();

            var sut = new CommandError(exception);

            sut.Message.Should().BeNull();
            sut.Code.Should().BeNull();
            sut.Exception.Should().Be(exception);
            sut.ToString().Should().Be($"{exception}");
        }
    }
}
