using CommandLine;
using FSLinkCommand;

#nullable enable

namespace FSLink.CommandLine
{
    public interface ICommandLineProcessor
    {
        ICommandBase? Process(string[] args);
    }

    public class CommandLineProcessor : ICommandLineProcessor
    {
        private readonly ICommandFactory _commandFactory;

        public CommandLineProcessor(ICommandFactory commandFactory)
        {
            _commandFactory = commandFactory;
        }

        public ICommandBase? Process(string[] args)
        {
            var result = Parser.Default.ParseArguments<ScanArguments, CreateArguments, DeleteArguments, RelinkArguments, ReparseArguments>(args)
                                       .MapResult<ScanArguments, CreateArguments, DeleteArguments, RelinkArguments, ReparseArguments, ICommandBase?>(
                                          (ScanArguments a) => _commandFactory.Create(a),
                                          (CreateArguments a) => _commandFactory.Create(a),
                                          (DeleteArguments a) => _commandFactory.Create(a),
                                          (RelinkArguments a) => _commandFactory.Create(a),
                                          (ReparseArguments a) => _commandFactory.Create(a),
                                          errs => null);
            return result;
        }
    }
}
