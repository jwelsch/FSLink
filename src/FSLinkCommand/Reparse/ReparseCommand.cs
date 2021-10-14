﻿using FSLinkLib;
using System.IO;
using System.Threading.Tasks;

namespace FSLinkCommand.Reparse
{
    public interface IReparseArguments : ICommandArguments
    {
        string Path { get; }
    }

    public class ReparseCommand : ICommandBase
    {
        private readonly IReparseArguments _commandArguments;
        private readonly IReparseOutput _commandOutput;
        private readonly IFileSystemLink _fileSystemLink;

        public string Name => "Reparse";

        public ReparseCommand(IReparseArguments commandArguments, IReparseOutput commandOutput, IFileSystemLink fileSystemLink)
        {
            _commandArguments = commandArguments;
            _fileSystemLink = fileSystemLink;
            _commandOutput = commandOutput;
        }

        public async Task<ICommandResult> Run()
        {
            return await Task.Run(() =>
            {
                var reparsePoint = _fileSystemLink.GetReparsePoint(_commandArguments.Path);

                if (reparsePoint == null)
                {
                    return new ErrorCommandResult(Name, new IOException($"Failed to get reparse point data for path '{_commandArguments.Path}'"));
                }

                _commandOutput.OnReparsePointData(reparsePoint);

                return (ICommandResult)new SuccessCommandResult(Name, reparsePoint);
            });
        }
    }
}
