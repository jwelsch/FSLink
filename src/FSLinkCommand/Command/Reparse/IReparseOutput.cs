using FSLinkLib.ReparsePoints;

#nullable enable

namespace FSLinkCommand.Command.Reparse
{
    public interface IReparseOutput
    {
        void OnReparsePointData(IReparsePoint reparsePoint);
    }
}
