namespace FSLinkLib.PInvoke
{
    internal class Constants
    {
        // This size comes from measuring the size of the header of REPARSE_GUID_DATA_BUFFER
        public const int REPARSE_GUID_DATA_BUFFER_HEADER_SIZE = 24;

        // Maximum reparse buffer info size. The max user defined reparse
        // data is 16KB, plus there's a header.
        public const int MAX_REPARSE_SIZE = (16 * 1024) + REPARSE_GUID_DATA_BUFFER_HEADER_SIZE;

        public const int FSCTL_GET_REPARSE_POINT = 0x000900A8;

        public const int FSCTL_SET_REPARSE_POINT = 0x000900A4;

        public const int FSCTL_DELETE_REPARSE_POINT = 0x000900AC;

        public const uint IO_REPARSE_TAG_SYMLINK = 0xA000000C;

        public const uint IO_REPARSE_TAG_MOUNT_POINT = 0xA0000003;

        public const uint IO_REPARSE_TAG_APPEXECLINK = 0x8000001B;

        public const string NonInterpretedPathPrefix = @"\??\";

        public const int MAX_PATH = 260;

        public const int ERROR_SUCCESS = 0;
        public const int ERROR_NOT_A_REPARSE_POINT = 0x1126; // 4390
    }
}
