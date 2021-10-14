using FSLinkLib;

namespace FSLinkCommand.Reparse
{
    public interface IReparseOutput
    {
        void OnReparsePointData(IReparsePoint reparsePoint);
    }
}
