namespace FSLinkLib
{
    //
    // Reparse tags are stored as DWORD values. The bits define certain attributes, as shown in the following diagram.
    // 
    // 3 3 2 2 2 2 2 2 2 2 2 2 1 1 1 1 1 1 1 1 1 1
    // 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0
    // +-+-+-+-+-----------------------+-------------------------------+
    // |M|R|N|R|     Reserved bits     |      Reparse tag value        |
    // +-+-+-+-+-----------------------+-------------------------------+
    //
    // The low 16 bits determine the kind of reparse point. The high 16 bits have 12 bits reserved for future use and
    // 4 bits that denote specific attributes of the tags and the data represented by the reparse point. The following
    // table describes these bits.
    //
    // Bit | Description
    // ----+------------------------------------------------------------------------------------------------------------
    //  M  | Microsoft bit. If this bit is set, the tag is owned by Microsoft. All other tags must use zero for this bit.
    //  R  | Reserved; must be zero for all non-Microsoft tags.
    //  N  | Name surrogate bit. If this bit is set, the file or directory represents another named entity in the system.
    //

    public interface IReparseTag
    {
        bool IsMicrosoft { get; }

        bool IsNameSurrogate { get; }

        bool ReservedFlag0 { get; }

        bool ReservedFlag1 { get; }

        ushort ReservedBits { get; }

        ushort TagValue { get; }
    }

    internal class ReparseTag : IReparseTag
    {
        public const uint MicrosoftMask = 0x80000000;     // 1000 0000 0000 0000 0000 0000 0000 0000
        public const uint ReservedFlag0Mask = 0x40000000; // 0100 0000 0000 0000 0000 0000 0000 0000
        public const uint NameSurrogateMask = 0x20000000; // 0010 0000 0000 0000 0000 0000 0000 0000
        public const uint ReservedFlag1Mask = 0x10000000; // 0001 0000 0000 0000 0000 0000 0000 0000
        public const uint ReservedBitsMask = 0x0FFF0000;  // 0000 1111 1111 1111 0000 0000 0000 0000
        public const uint TagValueMask = 0x0000FFFF;      // 0000 0000 0000 0000 1111 1111 1111 1111

        private readonly uint _data;

        public bool IsMicrosoft => (MicrosoftMask & _data) != 0;
        public bool IsNameSurrogate => (NameSurrogateMask & _data) != 0;
        public bool ReservedFlag0 => (ReservedFlag0Mask & _data) != 0;
        public bool ReservedFlag1 => (ReservedFlag1Mask & _data) != 0;
        public ushort ReservedBits => (ushort)((ReservedBitsMask & _data) >> 16);
        public ushort TagValue => (ushort)(TagValueMask & _data);

        public ReparseTag(uint data)
        {
            _data = data;
        }
    }
}
