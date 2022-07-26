namespace FSLinkLib.PInvoke
{
    internal class Constants
    {
        public const int REPARSE_DATA_BUFFER_HEADER_SIZE = 8;

        // This size comes from measuring the size of the header of REPARSE_GUID_DATA_BUFFER
        public const int REPARSE_GUID_DATA_BUFFER_HEADER_SIZE = 24;

        // Maximum reparse buffer info size. The max user defined reparse
        // data is 16KB.
        public const int MAX_REPARSE_SIZE = 16 * 1024;

        public const int FSCTL_GET_REPARSE_POINT = 0x000900A8;

        public const int FSCTL_SET_REPARSE_POINT = 0x000900A4;

        public const int FSCTL_DELETE_REPARSE_POINT = 0x000900AC;

        public const uint IO_REPARSE_TAG_SYMLINK = 0xA000000C;

        public const uint IO_REPARSE_TAG_MOUNT_POINT = 0xA0000003;

        public const uint IO_REPARSE_TAG_APPEXECLINK = 0x8000001B;

        public const string NonInterpretedPathPrefix = @"\??\";

        public const int MAX_PATH = 260;

        public const int ERROR_SUCCESS = 0;
        public const int ERROR_ACCESS_DENIED = 5;
        public const int ERROR_NOT_A_REPARSE_POINT = 0x1126; // 4390

        public const int ANYSIZE_ARRAY = 1;

        // DesiredAccess constants
        public const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        public const uint STANDARD_RIGHTS_READ = 0x00020000;
        public const uint TOKEN_ASSIGN_PRIMARY = 0x0001;
        public const uint TOKEN_DUPLICATE = 0x0002;
        public const uint TOKEN_IMPERSONATE = 0x0004;
        public const uint TOKEN_QUERY = 0x0008;
        public const uint TOKEN_QUERY_SOURCE = 0x0010;
        public const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
        public const uint TOKEN_ADJUST_GROUPS = 0x0040;
        public const uint TOKEN_ADJUST_DEFAULT = 0x0080;
        public const uint TOKEN_ADJUST_SESSIONID = 0x0100;
        public const uint TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);
        public const uint TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY |
            TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE |
            TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT |
            TOKEN_ADJUST_SESSIONID);

        // Security entities
        public const string SE_BACKUP_NAME = "SeBackupPrivilege";
        public const string SE_RESTORE_NAME = "SeRestorePrivilege";

        public const uint SE_PRIVILEGE_ENABLED = 0x00000002;
    }
}
