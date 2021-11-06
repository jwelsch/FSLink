using AutoFixture;
using FluentAssertions;
using FSLinkCommon.Util;
using System.Linq;
using Xunit;

namespace FSLinkCommon.Tests.Util
{
    public class ExtensionsTests
    {
        private static readonly Fixture AutoFixture = new();

        [Fact]
        public void When_tohexblock_given_arguments_to_fill_one_group_then_return_is_correct()
        {
            var value = AutoFixture.CreateMany<byte>(8).ToArray();

            var text1 = System.Text.Encoding.Unicode.GetString(value, 0, 8);

            var result = value.ToHexBlock();

            result.Should().Be($"010:  {value[0]:X2} {value[1]:X2} {value[2]:X2} {value[3]:X2} {value[4]:X2} {value[5]:X2} {value[6]:X2} {value[7]:X2}                           {text1}");
        }

        [Fact]
        public void When_tohexblock_given_arguments_to_fill_two_groups_then_return_is_correct()
        {
            var value = AutoFixture.CreateMany<byte>(16).ToArray();

            var text1 = System.Text.Encoding.Unicode.GetString(value, 0, 16);

            var result = value.ToHexBlock();

            result.Should().Be($"010:  {value[0]:X2} {value[1]:X2} {value[2]:X2} {value[3]:X2} {value[4]:X2} {value[5]:X2} {value[6]:X2} {value[7]:X2}"
                             + "  "
                             + $"{value[8]:X2} {value[9]:X2} {value[10]:X2} {value[11]:X2} {value[12]:X2} {value[13]:X2} {value[14]:X2} {value[15]:X2}"
                             + "  "
                             + $"{text1}");
        }

        [Fact]
        public void When_tohexblock_given_arguments_to_fill_two_rows_then_return_is_correct()
        {
            var value = AutoFixture.CreateMany<byte>(32).ToArray();

            var text1 = System.Text.Encoding.Unicode.GetString(value, 0, 16);
            var text2 = System.Text.Encoding.Unicode.GetString(value, 16, 16);

            var result = value.ToHexBlock();

            result.Should().Be($"010:  {value[0]:X2} {value[1]:X2} {value[2]:X2} {value[3]:X2} {value[4]:X2} {value[5]:X2} {value[6]:X2} {value[7]:X2}"
                             + "  "
                             + $"{value[8]:X2} {value[9]:X2} {value[10]:X2} {value[11]:X2} {value[12]:X2} {value[13]:X2} {value[14]:X2} {value[15]:X2}"
                             + "  "
                             + $"{text1}"
                             + "\r\n"
                             + $"020:  {value[16]:X2} {value[17]:X2} {value[18]:X2} {value[19]:X2} {value[20]:X2} {value[21]:X2} {value[22]:X2} {value[23]:X2}"
                             + "  "
                             + $"{value[24]:X2} {value[25]:X2} {value[26]:X2} {value[27]:X2} {value[28]:X2} {value[29]:X2} {value[30]:X2} {value[31]:X2}"
                             + "  "
                             + $"{text2}");
        }

        [Fact]
        public void When_tohexblock_given_arguments_to_fill_two_full_rows_and_one_partial_row_then_return_is_correct()
        {
            var value = new byte[] {
                0x5C, 0x00, 0x3F, 0x00, 0x3F, 0x00, 0x5C, 0x00,
                0x43, 0x00, 0x3A, 0x00, 0x5C, 0x00, 0x54, 0x00,
                0x65, 0x00, 0x6D, 0x00, 0x70, 0x00, 0x5C, 0x00,
                0x4C, 0x00, 0x69, 0x00, 0x6E, 0x00, 0x6B, 0x00,
                0x5C, 0x00, 0x54, 0x00, 0x61, 0x00, 0x72, 0x00,
                0x67, 0x00, 0x65, 0x00, 0x74, 0x00
            };

            var text1 = System.Text.Encoding.Unicode.GetString(value, 0, 16);
            var text2 = System.Text.Encoding.Unicode.GetString(value, 16, 16);
            var text3 = System.Text.Encoding.Unicode.GetString(value, 32, value.Length - 32);

            var result = value.ToHexBlock();

            result.Should().Be($"010:  {value[0]:X2} {value[1]:X2} {value[2]:X2} {value[3]:X2} {value[4]:X2} {value[5]:X2} {value[6]:X2} {value[7]:X2}" + "  " + $"{value[8]:X2} {value[9]:X2} {value[10]:X2} {value[11]:X2} {value[12]:X2} {value[13]:X2} {value[14]:X2} {value[15]:X2}" + $"  " + $"{text1}" + "\r\n"
                             + $"020:  {value[16]:X2} {value[17]:X2} {value[18]:X2} {value[19]:X2} {value[20]:X2} {value[21]:X2} {value[22]:X2} {value[23]:X2}" + "  " + $"{value[24]:X2} {value[25]:X2} {value[26]:X2} {value[27]:X2} {value[28]:X2} {value[29]:X2} {value[30]:X2} {value[31]:X2}" + $"  " + $"{text2}" + "\r\n"
                             + $"030:  {value[32]:X2} {value[33]:X2} {value[34]:X2} {value[35]:X2} {value[36]:X2} {value[37]:X2} {value[38]:X2} {value[39]:X2}" + "  " + $"{value[40]:X2} {value[41]:X2} {value[42]:X2} {value[43]:X2} {value[44]:X2} {value[45]:X2}      " + $"  " + $"{text3}");
        }
    }
}
