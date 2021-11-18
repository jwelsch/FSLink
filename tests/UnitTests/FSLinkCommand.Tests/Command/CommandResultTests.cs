using Autofac.Extras.NSubstitute;
using AutoFixture;
using FluentAssertions;
using FSLinkCommand.Command;
using System;
using Xunit;

namespace FSLinkCommand.Tests.Command
{
    public class CommandResultTests
    {
        private readonly static Fixture AutoFixture = new();

        [Fact]
        public void When_success_string_ctor_then_properties_are_as_expected()
        {
            using var autoSub = new AutoSubstitute();

            var name = AutoFixture.Create<string>();

            var sut = new SuccessCommandResult(name);

            sut.Success.Should().BeTrue();
            sut.CommandName.Should().Be(name);
            sut.Error.Should().BeNull();
            sut.Data.Should().BeNull();
        }

        [Fact]
        public void When_success_string_object_ctor_then_properties_are_as_expected()
        {
            using var autoSub = new AutoSubstitute();

            var name = AutoFixture.Create<string>();
            var data = AutoFixture.Create<object>();

            var sut = new SuccessCommandResult(name, data);

            sut.Success.Should().BeTrue();
            sut.CommandName.Should().Be(name);
            sut.Error.Should().BeNull();
            sut.Data.Should().Be(data);
        }
        [Fact]
        public void When_error_string_commanderror_ctor_then_properties_are_as_expected()
        {
            using var autoSub = new AutoSubstitute();

            var name = AutoFixture.Create<string>();
            var error = AutoFixture.Create<CommandError>();

            var sut = new ErrorCommandResult(name, error);

            sut.Success.Should().BeFalse();
            sut.CommandName.Should().Be(name);
            sut.Error.Should().Be(error);
            sut.Data.Should().BeNull();
        }

        [Fact]
        public void When_error_string_string_ctor_then_properties_are_as_expected()
        {
            using var autoSub = new AutoSubstitute();

            var name = AutoFixture.Create<string>();
            var message = AutoFixture.Create<string>();

            var sut = new ErrorCommandResult(name, message);

            sut.Success.Should().BeFalse();
            sut.CommandName.Should().Be(name);
            sut.Error.Message.Should().Be(message);
            sut.Error.Code.Should().BeNull();
            sut.Error.Exception.Should().BeNull();
            sut.Data.Should().BeNull();
        }

        [Fact]
        public void When_error_string_string_int_ctor_then_properties_are_as_expected()
        {
            using var autoSub = new AutoSubstitute();

            var name = AutoFixture.Create<string>();
            var message = AutoFixture.Create<string>();
            var code = AutoFixture.Create<int>();

            var sut = new ErrorCommandResult(name, message, code);

            sut.Success.Should().BeFalse();
            sut.CommandName.Should().Be(name);
            sut.Error.Message.Should().Be(message);
            sut.Error.Code.Should().Be(code);
            sut.Error.Exception.Should().BeNull();
            sut.Data.Should().BeNull();
        }

        [Fact]
        public void When_error_string_string_exception_ctor_then_properties_are_as_expected()
        {
            using var autoSub = new AutoSubstitute();

            var name = AutoFixture.Create<string>();
            var message = AutoFixture.Create<string>();
            var exception = AutoFixture.Create<Exception>();

            var sut = new ErrorCommandResult(name, message, exception);

            sut.Success.Should().BeFalse();
            sut.CommandName.Should().Be(name);
            sut.Error.Message.Should().Be(message);
            sut.Error.Code.Should().BeNull();
            sut.Error.Exception.Should().Be(exception);
            sut.Data.Should().BeNull();
        }

        [Fact]
        public void When_error_string_exception_ctor_then_properties_are_as_expected()
        {
            using var autoSub = new AutoSubstitute();

            var name = AutoFixture.Create<string>();
            var exception = AutoFixture.Create<Exception>();

            var sut = new ErrorCommandResult(name, exception);

            sut.Success.Should().BeFalse();
            sut.CommandName.Should().Be(name);
            sut.Error.Message.Should().BeNull();
            sut.Error.Code.Should().BeNull();
            sut.Error.Exception.Should().Be(exception);
            sut.Data.Should().BeNull();
        }
    }
}
