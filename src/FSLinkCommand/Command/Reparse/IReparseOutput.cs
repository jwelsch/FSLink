using FSLinkLib;

namespace FSLinkCommand.Command.Reparse
{
    public interface IReparseOutput
    {
        void OnReparsePointData(IReparsePoint reparsePoint);
    }
}
