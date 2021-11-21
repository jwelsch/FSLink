using AutoFixture;
using FluentAssertions;
using FSLinkLib.ReparsePoints;
using Xunit;

namespace FSLinkLib.Tests.ReparsePoints
{
    public class ReparseTagTests
    {
        private readonly static Fixture AutoFixture = new();

        [Fact]
        public void When_data_is_given_to_ctor_then_rawdata_property_is_as_expected()
        {
            var data = AutoFixture.Create<uint>();

            var sut = new ReparseTag(data);

            sut.RawData.Should().Be(data);
        }

        [Fact]
        public void When_data_given_to_ctor_is_microsoft_then_ismicrosoft_property_is_true()
        {
            var data = 0x80000000U;

            var sut = new ReparseTag(data);

            sut.IsMicrosoft.Should().BeTrue();
        }

        [Fact]
        public void When_data_given_to_ctor_is_not_microsoft_then_ismicrosoft_property_is_false()
        {
            var data = ~0x80000000U;

            var sut = new ReparseTag(data);

            sut.IsMicrosoft.Should().BeFalse();
        }

        [Fact]
        public void When_data_given_to_ctor_has_reserve_flag_0_then_reserveflag0_property_is_true()
        {
            var data = 0x40000000U;

            var sut = new ReparseTag(data);

            sut.ReservedFlag0.Should().BeTrue();
        }

        [Fact]
        public void When_data_given_to_ctor_does_not_have_reserve_flag_0_then_reserveflag0_property_is_false()
        {
            var data = ~0x40000000U;

            var sut = new ReparseTag(data);

            sut.ReservedFlag0.Should().BeFalse();
        }

        [Fact]
        public void When_data_given_to_ctor_is_name_surrogate_then_isnamesurrogate_property_is_true()
        {
            var data = 0x20000000U;

            var sut = new ReparseTag(data);

            sut.IsNameSurrogate.Should().BeTrue();
        }

        [Fact]
        public void When_data_given_to_ctor_is_not_name_surrogate_then_isnamesurrogate_property_is_false()
        {
            var data = ~0x20000000U;

            var sut = new ReparseTag(data);

            sut.IsNameSurrogate.Should().BeFalse();
        }

        [Fact]
        public void When_data_given_to_ctor_has_reserve_flag_1_then_reserveflag1_property_is_true()
        {
            var data = 0x10000000U;

            var sut = new ReparseTag(data);

            sut.ReservedFlag1.Should().BeTrue();
        }

        [Fact]
        public void When_data_given_to_ctor_does_not_have_reserve_flag_1_then_reserveflag1_property_is_false()
        {
            var data = ~0x10000000U;

            var sut = new ReparseTag(data);

            sut.ReservedFlag1.Should().BeFalse();
        }

        [Fact]
        public void When_data_given_to_ctor_has_reserved_bits_then_reservedbits_property_is_expected_value()
        {
            var data = 0x0FFF0000U;

            var sut = new ReparseTag(data);

            sut.ReservedBits.Should().Be(0x0FFF);
        }

        [Fact]
        public void When_data_given_to_ctor_does_not_have_reserved_bits_then_reservedbits_property_is_zero()
        {
            var data = ~0x0FFF0000U;

            var sut = new ReparseTag(data);

            sut.ReservedBits.Should().Be(0x0);
        }

        [Fact]
        public void When_data_given_to_ctor_has_tag_value_then_tagvalue_property_is_expected_value()
        {
            var data = 0x0000FFFFU;

            var sut = new ReparseTag(data);

            sut.TagValue.Should().Be(0xFFFF);
        }

        [Fact]
        public void When_data_given_to_ctor_does_not_have_tag_value_then_tagvalue_property_is_zero()
        {
            var data = ~0x0000FFFFU;

            var sut = new ReparseTag(data);

            sut.TagValue.Should().Be(0x0);
        }
    }
}
