using System.IO;

namespace FSLinkCommon.Wraps
{
    public interface IDirectoryInfoWrap
    {
    }

    public class DirectoryInfoWrap : IDirectoryInfoWrap
    {
        private DirectoryInfo _directoryInfo;

        public DirectoryInfoWrap(DirectoryInfo directoryInfo)
        {
            _directoryInfo = directoryInfo;
        }
    }
}
