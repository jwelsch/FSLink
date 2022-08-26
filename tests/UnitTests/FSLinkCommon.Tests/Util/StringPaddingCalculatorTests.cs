using Autofac.Extras.NSubstitute;
using AutoFixture;
using FluentAssertions;
using FSLinkCommon.Util;
using System;
using System.Linq;
using Xunit;

namespace FSLinkCommon.Tests.Util
{
    public class StringPaddingCalculatorTests
    {
        private readonly AutoSubstitute _container = new();
        private readonly Fixture _autoFixture = new();

        [Fact]
        public void When_value_string_length_is_less_than_target_length_then_return_padded_string()
        {
            var value = _autoFixture.Create<string>();
            var targetLength = value.Length + 1;

            var sut = _container.Resolve<StringPaddingCalculator>();

            var result = sut.Calculate(value, targetLength);

            result.Should().Be($"{value} ");
        }

        [Fact]
        public void When_custom_padding_character_specified_then_return_padded_string()
        {
            var value = _autoFixture.Create<string>();
            var targetLength = value.Length + 1;
            var padding = '~';

            var sut = _container.Resolve<StringPaddingCalculator>();

            var result = sut.Calculate(value, targetLength, padding);

            result.Should().Be($"{value}{padding}");
        }

        [Fact]
        public void When_value_string_length_is_equal_to_target_length_then_return_string()
        {
            var value = _autoFixture.Create<string>();
            var targetLength = value.Length;

            var sut = _container.Resolve<StringPaddingCalculator>();

            var result = sut.Calculate(value, targetLength);

            result.Should().Be(value);
        }

        [Fact]
        public void When_value_string_length_is_greater_than_target_length_and_option_is_truncate_then_return_truncated_string()
        {
            var value = _autoFixture.Create<string>();
            var targetLength = value.Length - 1;

            var sut = _container.Resolve<StringPaddingCalculator>();

            var result = sut.Calculate(value, targetLength, ' ', StringPaddingCalculatorOptions.Truncate);

            result.Should().Be(value.Substring(0, value.Length - 1));
        }

        [Fact]
        public void When_value_string_length_is_greater_than_target_length_and_option_is_expand_then_return_unmodified_string()
        {
            var value = _autoFixture.Create<string>();
            var targetLength = value.Length - 1;

            var sut = _container.Resolve<StringPaddingCalculator>();

            var result = sut.Calculate(value, targetLength, ' ', StringPaddingCalculatorOptions.Expand);

            result.Should().Be(value);
        }

        [Fact]
        public void When_value_string_length_is_greater_than_target_length_and_option_is_throwexception_then_throw_argumentexception()
        {
            var value = _autoFixture.Create<string>();
            var targetLength = value.Length - 1;

            var sut = _container.Resolve<StringPaddingCalculator>();

            Action action = () => sut.Calculate(value, targetLength, ' ', StringPaddingCalculatorOptions.ThrowException);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void When_target_length_is_less_than_zero_then_throw_argumentexception()
        {
            var value = _autoFixture.Create<string>();
            var targetLength = -1;

            var sut = _container.Resolve<StringPaddingCalculator>();

            Action action = () => sut.Calculate(value, targetLength);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void When_value_string_length_is_zero_and_target_length_is_zero_then_return_empty_string()
        {
            var value = string.Empty;
            var targetLength = value.Length;

            var sut = _container.Resolve<StringPaddingCalculator>();

            var result = sut.Calculate(value, targetLength);

            result.Should().Be(value);
        }

        [Fact]
        public void When_value_string_length_is_zero_and_target_length_is_valid_then_return_padded_string()
        {
            var value = string.Empty;
            var targetLength = 10;
            var padding = ' ';

            var sut = _container.Resolve<StringPaddingCalculator>();

            var result = sut.Calculate(value, targetLength, padding);

            result.Should().Be(new string(padding, targetLength));
        }

        [Fact]
        public void When_empty_values_enumerable_then_throw_argumentexception()
        {
            var value = _autoFixture.Create<string>();
            var values = _autoFixture.CreateMany<string>(0);

            var sut = _container.Resolve<StringPaddingCalculator>();

            Action action = () => sut.Calculate(value, values);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void When_calculate_with_values_and_custom_padding_character_specified_then_return_padded_string()
        {
            var value = _autoFixture.Create<string>()[..^1];
            var values = _autoFixture.CreateMany<string>(1);
            var padding = '~';

            var sut = _container.Resolve<StringPaddingCalculator>();

            var result = sut.Calculate(value, values, padding);

            result.Should().Be($"{value}{padding}");
        }

        [Fact]
        public void When_value_string_length_is_less_than_target_length_and_values_enumerable_has_one_item_then_return_padded_string()
        {
            var value = _autoFixture.Create<string>()[..^1];
            var values = _autoFixture.CreateMany<string>(1);

            var sut = _container.Resolve<StringPaddingCalculator>();

            var result = sut.Calculate(value, values);

            result.Should().Be($"{value}{new string(' ', 1)}");
        }

        [Fact]
        public void When_value_string_length_is_less_than_target_length_and_values_enumerable_has_multiple_items_then_return_padded_string()
        {
            var value = _autoFixture.Create<string>();
            var values = new string[]
            {
                _autoFixture.Create<string>() + _autoFixture.Create<string>(),
                _autoFixture.Create<string>()[..^1]
            };

            var sut = _container.Resolve<StringPaddingCalculator>();

            var result = sut.Calculate(value, values);

            result.Should().Be($"{value}{new string(' ', values[0].Length - value.Length)}");
        }

        [Fact]
        public void When_value_string_length_is_equal_to_target_length_and_values_enumerable_has_one_item_then_return_string()
        {
            var value = _autoFixture.Create<string>();
            var values = _autoFixture.CreateMany<string>(1);

            var sut = _container.Resolve<StringPaddingCalculator>();

            var result = sut.Calculate(value, values);

            result.Should().Be(value);
        }

        [Fact]
        public void When_value_string_length_is_equal_to_target_length_and_values_enumerable_has_multiple_items_then_return_string()
        {
            var value = _autoFixture.Create<string>();
            var values = new string[]
            {
                _autoFixture.Create<string>(),
                _autoFixture.Create<string>()[..^1]
            };

            var sut = _container.Resolve<StringPaddingCalculator>();

            var result = sut.Calculate(value, values);

            result.Should().Be(value);
        }

        [Fact]
        public void When_value_string_length_is_greater_than_target_length_and_values_enumerable_has_one_item_and_option_is_truncate_then_return_truncated_string()
        {
            var value = _autoFixture.Create<string>() + _autoFixture.Create<string>();
            var values = _autoFixture.CreateMany<string>(1);

            var sut = _container.Resolve<StringPaddingCalculator>();

            var result = sut.Calculate(value, values, ' ', StringPaddingCalculatorOptions.Truncate);

            result.Should().Be(value[..values.First().Length]);
        }

        [Fact]
        public void When_value_string_length_is_greater_than_target_length_and_values_enumerable_has_multiple_items_and_option_is_truncate_then_return_truncated_string()
        {
            var value = _autoFixture.Create<string>() + _autoFixture.Create<string>();
            var values = new string[]
            {
                _autoFixture.Create<string>(),
                _autoFixture.Create<string>()[..^1]
            };

            var sut = _container.Resolve<StringPaddingCalculator>();

            var result = sut.Calculate(value, values, ' ', StringPaddingCalculatorOptions.Truncate);

            result.Should().Be(value[..values.First().Length]);
        }

        [Fact]
        public void When_value_string_length_is_greater_than_target_length_and_values_enumerable_has_one_item_and_option_is_expand_then_return_string()
        {
            var value = _autoFixture.Create<string>() + _autoFixture.Create<string>();
            var values = _autoFixture.CreateMany<string>(1);

            var sut = _container.Resolve<StringPaddingCalculator>();

            var result = sut.Calculate(value, values, ' ', StringPaddingCalculatorOptions.Expand);

            result.Should().Be(value);
        }

        [Fact]
        public void When_value_string_length_is_greater_than_target_length_and_values_enumerable_has_multiple_items_and_option_is_expand_then_return_string()
        {
            var value = _autoFixture.Create<string>() + _autoFixture.Create<string>();
            var values = new string[]
            {
                _autoFixture.Create<string>(),
                _autoFixture.Create<string>()[..^1]
            };

            var sut = _container.Resolve<StringPaddingCalculator>();

            var result = sut.Calculate(value, values, ' ', StringPaddingCalculatorOptions.Expand);

            result.Should().Be(value);
        }

        [Fact]
        public void When_value_string_length_is_greater_than_target_length_and_values_enumerable_has_one_item_and_option_is_throwexception_then_throw_argumentexception()
        {
            var value = _autoFixture.Create<string>() + _autoFixture.Create<string>();
            var values = _autoFixture.CreateMany<string>(1);

            var sut = _container.Resolve<StringPaddingCalculator>();

            Action action = () => sut.Calculate(value, values, ' ', StringPaddingCalculatorOptions.ThrowException);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void When_value_string_length_is_greater_than_target_length_and_values_enumerable_has_multiple_items_and_option_is_throwexception_then_throw_argumentexception()
        {
            var value = _autoFixture.Create<string>() + _autoFixture.Create<string>();
            var values = new string[]
            {
                _autoFixture.Create<string>(),
                _autoFixture.Create<string>()[..^1]
            };

            var sut = _container.Resolve<StringPaddingCalculator>();

            Action action = () => sut.Calculate(value, values, ' ', StringPaddingCalculatorOptions.ThrowException);

            action.Should().Throw<ArgumentException>();
        }
    }
}
