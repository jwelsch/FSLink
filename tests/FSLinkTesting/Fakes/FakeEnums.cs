using System;

namespace FSLinkTesting.Fakes
{
    public enum EnumWithNoValues
    {
    }

    public enum EnumWithOneValue
    {
        Only
    }

    public enum EnumWithMultipleValues
    {
        First,
        Second,
        Third
    }

    [Flags]
    public enum EnumWithFlagsAttribute
    {
        First,
        Second,
        Third
    }
}
