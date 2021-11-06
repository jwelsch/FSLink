#nullable enable

using System;
using System.Text;

namespace FSLinkCommon.Util
{
    public static class Extensions
    {
        public static string ToHexString(this byte value)
        {
            return $"0x{value:X2}";
        }

        public static string ToHexString(this ushort value)
        {
            return $"0x{value:X4}";
        }

        public static string ToHexString(this uint value)
        {
            return $"0x{value:X8}";
        }

        public static string ToHexString(this ulong value)
        {
            return $"0x{value:X16}";
        }

        public static string ToHexBlock(this byte[] value, int start = 0, int length = -1, int octetsPerGroup = 8, int groupsPerRow = 2)
        {
            if (start < 0 || start >= value.Length)
            {
                throw new ArgumentException($"Must be greater than or equal to zero and less than {value.Length}.", nameof(start));
            }

            if (length < 0)
            {
                length = value.Length;
            }

            if (start + length > value.Length)
            {
                throw new ArgumentException($"When added to 'start' ({start}), must not be greater than {value.Length}.", nameof(length));
            }

            if (octetsPerGroup <= 0)
            {
                throw new ArgumentException($"Must be greater than zero.", nameof(octetsPerGroup));
            }

            if (groupsPerRow <= 0)
            {
                throw new ArgumentException($"Must be greater than zero.", nameof(groupsPerRow));
            }

            if (value.Length == 0)
            {
                return string.Empty;
            }

            var mainBuilder = new StringBuilder();

            var octetsPerRow = octetsPerGroup * groupsPerRow;
            var totalRows = length / octetsPerRow + (length % octetsPerRow == 0 ? 0 : 1);
            var charsPerRow = ((3 * octetsPerGroup - 1) * groupsPerRow) + (2 * groupsPerRow - 2);


            var digitsInOctetCount = 0;
            var lengthCalculator = length;

            do
            {
                lengthCalculator /= 16;
                digitsInOctetCount++;
            }
            while (lengthCalculator > 0);

            if (digitsInOctetCount < 3)
            {
                digitsInOctetCount = 3;
            }

            for (int row = 0, i = 0; i < length; row++)
            {
                if (row != 0)
                {
                    mainBuilder.AppendLine();
                }

                mainBuilder.Append($"{(octetsPerRow * (row + 1)).ToString($"X{digitsInOctetCount}")}:  ");

                var charBuilder = new StringBuilder();
                var charsWrittenInRow = 0;

                for (var group = 0; group < groupsPerRow && i < length; group++)
                {
                    if (group != 0)
                    {
                        mainBuilder.Append(' ', 2);
                        charsWrittenInRow += 2;
                    }

                    for (var octet = start; octet < octetsPerGroup && i < length; octet++)
                    {
                        string padding = "";
                        if (octet != 0)
                        {
                            padding = " ";
                            charsWrittenInRow += 1;
                        }

                        mainBuilder.Append($"{padding}{value[i++]:X2}");

                        charsWrittenInRow += 2;

                        if (octet % 2 == 1)
                        {
                            var encoded = Encoding.Unicode.GetString(value, i - 2, 2);
                            charBuilder.Append(encoded);
                        }
                    }
                }

                if (charsPerRow > charsWrittenInRow)
                {
                    mainBuilder.Append(' ', charsPerRow - charsWrittenInRow);
                }

                mainBuilder.Append($"  {charBuilder}");
            }

            return mainBuilder.ToString();
        }
    }
}
