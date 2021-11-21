using Autofac.Extras.NSubstitute;
using FluentAssertions;
using FSLinkCommand.Command;
using FSLinkCommand.Tests.Fakes;
using System;
using System.Threading.Tasks;
using Xunit;

#nullable enable

namespace FSLinkCommand.Tests.Command
{
    public class CommandBaseTests
    {
        [Fact]
        public void When_ctor_called_then_properties_are_set_correctly()
        {
            using var autoSub = new AutoSubstitute();

            var sut = autoSub.Resolve<FakeCommand>();

            sut.Name.Should().Be("Fake");
        }

        [Fact]
        public async void When_run_called_with_correct_arguments_then_expected_result_returned()
        {
            using var autoSub = new AutoSubstitute();

            var arguments = autoSub.Resolve<IFakeArguments>();

            var sut = autoSub.Resolve<FakeCommand>();

            var result = await sut.Run(arguments);

            result.Should().BeOfType(typeof(SuccessCommandResult));
        }

        [Fact]
        public void When_run_called_with_incorrect_arguments_then_throw_argumentexception()
        {
            using var autoSub = new AutoSubstitute();

            var arguments = autoSub.Resolve<IBadArguments>();

            var sut = autoSub.Resolve<FakeCommand>();

            Func<Task> func = async () => await sut.Run(arguments);

            func.Should().ThrowAsync<ArgumentException>();
        }
    }
}
