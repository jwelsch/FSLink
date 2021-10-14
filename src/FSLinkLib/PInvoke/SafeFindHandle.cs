using Microsoft.Win32.SafeHandles;

namespace FSLinkLib.PInvoke
{
    internal class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private readonly INativeMethodCaller _nativeMethodCaller;

        public SafeFindHandle(INativeMethodCaller nativeMethodCaller)
            : base(true)
        {
            _nativeMethodCaller = nativeMethodCaller;
        }

        protected override bool ReleaseHandle()
        {
            return _nativeMethodCaller.FindClose(this.handle);
        }
    }
}
