using System;
using System.Collections.Generic;
using System.Linq;

namespace FSLinkCommon.Util
{
    public enum StringPaddingCalculatorOptions
    {
        Truncate,
        Expand,
        ThrowException
    }

    public interface IStringPaddingCalculator
    {
        string Calculate<T, U>(T value, IEnumerable<U> values, char padding = ' ', StringPaddingCalculatorOptions options = StringPaddingCalculatorOptions.Truncate);

        string Calculate<T>(T value, int targetLength, char padding = ' ', StringPaddingCalculatorOptions options = StringPaddingCalculatorOptions.Truncate);
    }

    public class StringPaddingCalculator : IStringPaddingCalculator
    {
        public string Calculate<T, U>(T value, IEnumerable<U> values, char padding = ' ', StringPaddingCalculatorOptions options = StringPaddingCalculatorOptions.Truncate)
        {
            if (!values.Any())
            {
                throw new ArgumentException("The values enumerable cannot be empty.", nameof(values));
            }

            var lengths = values.Select(i => i.ToString().Length);
            var targetLength = lengths.Any() ? lengths.Max() : 0;
            return Calculate(value, targetLength, padding, options);
        }

        public string Calculate<T>(T value, int targetLength, char padding = ' ', StringPaddingCalculatorOptions options = StringPaddingCalculatorOptions.Truncate)
        {
            if (targetLength < 0)
            {
                throw new ArgumentException("The target length cannot be less than zero.", nameof(targetLength));
            }

            var valueString = value.ToString();

            var paddingCount = 0;

            if (valueString.Length > targetLength)
            {
                if (options == StringPaddingCalculatorOptions.Truncate)
                {
                    valueString = valueString[..targetLength];
                }
                else if (options != StringPaddingCalculatorOptions.Expand)
                {
                    throw new ArgumentException("The length of the value string exceeds the maximum allowed length.", nameof(value));
                }
            }
            else
            {
                paddingCount = targetLength - valueString.Length;
            }

            return $"{valueString}{new string(padding, paddingCount)}";
        }
    }
}
