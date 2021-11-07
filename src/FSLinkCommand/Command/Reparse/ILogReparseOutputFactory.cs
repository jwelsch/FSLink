using FSLinkLib.ReparsePoints;
using System;

#nullable enable

namespace FSLinkCommand.Command.Reparse
{
    public interface ILogReparseOutputFactory
    {
        IReparseOutput Create(IReparsePoint? reparsePoint);
    }
}
