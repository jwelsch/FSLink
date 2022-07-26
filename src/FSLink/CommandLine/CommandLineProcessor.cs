using CommandLine;
using FSLinkCommand.Command;
using System;

#nullable enable

namespace FSLink.CommandLine
{
    public interface ICommandLineProcessor
    {
        ICommandArguments? Process(string[] args);
    }

    public class CommandLineProcessor : ICommandLineProcessor
    {
        public ICommandArguments Process(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException($"No arguments were found.", nameof(args));
            }

            var result = Parser.Default.ParseArguments<ScanArguments, CreateArguments, DeleteArguments, RelinkArguments, ReparseArguments>(args)
                                       .MapResult<ScanArguments, CreateArguments, DeleteArguments, RelinkArguments, ReparseArguments, ICommandArguments>(
                                          (ScanArguments a) => a,
                                          (CreateArguments a) => a,
                                          (DeleteArguments a) => a,
                                          (RelinkArguments a) => a,
                                          (ReparseArguments a) => a,
                                          errs => throw new CommandLineException(errs));
            return result;
        }
    }
}
