using Autofac.Extras.NSubstitute;
using AutoFixture;
using FluentAssertions;
using FSLinkCommon.Util;
using FSLinkTesting.Fakes;
using System;
using System.Linq;
using Xunit;

namespace FSLinkCommon.Tests.Util
{
    public class EnumValueStringLengthCalculatorTests
    {
        private readonly AutoSubstitute _container = new();

        [Fact]
        public void When_calculate_given_enum_with_no_values_then_return_empty_enumerable()
        {
            var sut = _container.Resolve<EnumValueStringLengthCalculator>();

            var result = sut.Calculate<EnumWithNoValues>();

            result.Should().BeEmpty();
        }

        [Fact]
        public void When_calculate_given_enum_with_one_value_then_return_enumerable_with_one_item()
        {
            var sut = _container.Resolve<EnumValueStringLengthCalculator>();

            var result = sut.Calculate<EnumWithOneValue>();

            var expectedValues = Enum.GetValues<EnumWithOneValue>();

            result.Should().HaveCount(expectedValues.Length);
            result.First().Value.Should().Be(expectedValues.First());
            result.First().Length.Should().Be(expectedValues.First().ToString().Length);
        }

        [Fact]
        public void When_calculate_given_enum_with_multiple_values_then_return_enumerable_with_multiple_items()
        {
            var sut = _container.Resolve<EnumValueStringLengthCalculator>();

            var result = sut.Calculate<EnumWithMultipleValues>();

            var expectedValues = Enum.GetValues<EnumWithMultipleValues>();

            result.Should().HaveCount(expectedValues.Length);
            result.Skip(0).Take(1).First().Value.Should().Be(expectedValues.Skip(0).Take(1).First());
            result.Skip(0).Take(1).First().Length.Should().Be(expectedValues.Skip(0).Take(1).First().ToString().Length);
            result.Skip(1).Take(1).First().Value.Should().Be(expectedValues.Skip(1).Take(1).First());
            result.Skip(1).Take(1).First().Length.Should().Be(expectedValues.Skip(1).Take(1).First().ToString().Length);
            result.Skip(2).Take(1).First().Value.Should().Be(expectedValues.Skip(2).Take(1).First());
            result.Skip(2).Take(1).First().Length.Should().Be(expectedValues.Skip(2).Take(1).First().ToString().Length);
        }

        [Fact]
        public void When_calculate_given_enum_with_flags_attribute_then_return_enumerable_with_multiple_items()
        {
            var sut = _container.Resolve<EnumValueStringLengthCalculator>();

            var result = sut.Calculate<EnumWithFlagsAttribute>();

            var expectedValues = Enum.GetValues<EnumWithFlagsAttribute>();

            result.Should().HaveCount(expectedValues.Length);
            result.Skip(0).Take(1).First().Value.Should().Be(expectedValues.Skip(0).Take(1).First());
            result.Skip(0).Take(1).First().Length.Should().Be(expectedValues.Skip(0).Take(1).First().ToString().Length);
            result.Skip(1).Take(1).First().Value.Should().Be(expectedValues.Skip(1).Take(1).First());
            result.Skip(1).Take(1).First().Length.Should().Be(expectedValues.Skip(1).Take(1).First().ToString().Length);
            result.Skip(2).Take(1).First().Value.Should().Be(expectedValues.Skip(2).Take(1).First());
            result.Skip(2).Take(1).First().Length.Should().Be(expectedValues.Skip(2).Take(1).First().ToString().Length);
        }

        [Fact]
        public void When_getmaximumstringlength_given_enum_with_no_values_then_return_zero()
        {
            var sut = _container.Resolve<EnumValueStringLengthCalculator>();

            var result = sut.GetMaximumStringLength<EnumWithNoValues>();

            result.Should().Be(0);
        }

        [Fact]
        public void When_getmaximumstringlength_given_enum_with_one_value_then_return_maximum_length()
        {
            var sut = _container.Resolve<EnumValueStringLengthCalculator>();

            var result = sut.GetMaximumStringLength<EnumWithOneValue>();

            result.Should().Be(EnumWithOneValue.Only.ToString().Length);
        }

        [Fact]
        public void When_getmaximumstringlength_given_enum_with_multiple_values_then_return_maximum_length()
        {
            var sut = _container.Resolve<EnumValueStringLengthCalculator>();

            var result = sut.GetMaximumStringLength<EnumWithMultipleValues>();

            result.Should().Be(EnumWithMultipleValues.Second.ToString().Length);
        }

        [Fact]
        public void When_getmaximumstringlength_given_enum_with_flags_attribute_then_return_maximum_length()
        {
            var sut = _container.Resolve<EnumValueStringLengthCalculator>();

            var result = sut.GetMaximumStringLength<EnumWithFlagsAttribute>();

            result.Should().Be(EnumWithFlagsAttribute.Second.ToString().Length);
        }

        [Fact]
        public void When_getpaddedstring_given_enum_with_one_value_and_the_value_then_return_padded_string()
        {
            var sut = _container.Resolve<EnumValueStringLengthCalculator>();

            var result = sut.GetPaddedString(EnumWithOneValue.Only);

            result.Should().Be(EnumWithOneValue.Only.ToString());
        }

        [Fact]
        public void When_getpaddedstring_given_enum_with_multiple_values_and_shortest_value_then_return_padded_string()
        {
            var sut = _container.Resolve<EnumValueStringLengthCalculator>();

            var result = sut.GetPaddedString(EnumWithMultipleValues.First);

            result.Length.Should().Be(EnumWithMultipleValues.Second.ToString().Length);
            result.Should().Be($"{EnumWithMultipleValues.First} ");
        }

        [Fact]
        public void When_getpaddedstring_given_enum_with_multiple_values_and_longest_value_then_return_unpadded_string()
        {
            var sut = _container.Resolve<EnumValueStringLengthCalculator>();

            var result = sut.GetPaddedString(EnumWithMultipleValues.Second);

            result.Should().Be(EnumWithMultipleValues.Second.ToString());
        }

        [Fact]
        public void When_getpaddedstring_given_enum_with_flags_attribute_values_and_shortest_value_then_return_padded_string()
        {
            var sut = _container.Resolve<EnumValueStringLengthCalculator>();

            var result = sut.GetPaddedString(EnumWithFlagsAttribute.First);

            result.Length.Should().Be(EnumWithFlagsAttribute.Second.ToString().Length);
            result.Should().Be($"{EnumWithFlagsAttribute.First} ");
        }

        [Fact]
        public void When_getpaddedstring_given_enum_with_flags_attribute_values_and_longest_value_then_return_unpadded_string()
        {
            var sut = _container.Resolve<EnumValueStringLengthCalculator>();

            var result = sut.GetPaddedString(EnumWithFlagsAttribute.Second);

            result.Should().Be(EnumWithFlagsAttribute.Second.ToString());
        }

        [Fact]
        public void When_getpaddedstring_given_custom_pad_character_then_return_padded_string()
        {
            var padChar = '~';

            var sut = _container.Resolve<EnumValueStringLengthCalculator>();

            var result = sut.GetPaddedString(EnumWithMultipleValues.First, padChar);

            result.Length.Should().Be(EnumWithMultipleValues.Second.ToString().Length);
            result.Should().Be($"{EnumWithMultipleValues.First}{padChar}");
        }
    }
}
