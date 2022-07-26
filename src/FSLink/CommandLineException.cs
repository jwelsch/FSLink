using CommandLine;
using System;
using System.Collections.Generic;

namespace FSLink
{

    [Serializable]
    public class CommandLineException : Exception
    {
        public IReadOnlyList<Error> Errors { get; }

        public CommandLineException(IEnumerable<Error> errors)
        {
            Errors = new List<Error>(errors).AsReadOnly();
        }

        public CommandLineException(IEnumerable<Error> errors, string message)
            : base(message)
        {
            Errors = new List<Error>(errors).AsReadOnly();
        }

        public CommandLineException(IEnumerable<Error> errors, string message, Exception inner)
            : base(message, inner)
        {
            Errors = new List<Error>(errors).AsReadOnly();
        }

        protected CommandLineException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
