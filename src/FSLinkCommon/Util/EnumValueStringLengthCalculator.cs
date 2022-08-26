using System;
using System.Collections.Generic;
using System.Linq;

namespace FSLinkCommon.Util
{
    public interface IEnumStringLength<TEnum> where TEnum : struct, Enum
    {
        TEnum Value { get; }

        int Length { get; }
    }

    public class EnumStringLength<TEnum> : IEnumStringLength<TEnum> where TEnum : struct, Enum
    {
        public TEnum Value { get; }

        public int Length { get; }

        public EnumStringLength(TEnum value, int length)
        {
            Value = value;
            Length = length;
        }
    }

    public interface IEnumValueStringLengthCalculator
    {
        IEnumerable<IEnumStringLength<TEnum>> Calculate<TEnum>() where TEnum : struct, Enum;

        int GetMaximumStringLength<TEnum>() where TEnum : struct, Enum;

        string GetPaddedString<TEnum>(TEnum value, char padding = ' ') where TEnum : struct, Enum;
    }

    public class EnumValueStringLengthCalculator : IEnumValueStringLengthCalculator
    {
        public IEnumerable<IEnumStringLength<TEnum>> Calculate<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetValues<TEnum>()
                       .Select(i => new EnumStringLength<TEnum>(i, i.ToString().Length));
        }

        public int GetMaximumStringLength<TEnum>() where TEnum : struct, Enum
        {
            var values = Calculate<TEnum>();
            return values.Any() ? values.Max(i => i.Length) : 0;
        }

        public string GetPaddedString<TEnum>(TEnum value, char padding = ' ') where TEnum : struct, Enum
        {
            var values = Calculate<TEnum>();
            var maxLength = values.Max(i => i.Length);
            var valueLength = values.First(i => Equals(i.Value, value)).Length;

            return $"{value}{new string(padding, maxLength - valueLength)}";
        }
    }
}
