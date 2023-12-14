namespace Catel.Services
{
    using System;

    public class ProcessResult
    {
        public ProcessResult(ProcessContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            Context = context;
        }

        public ProcessContext Context { get; }

        public int ExitCode { get; set; }
    }
}
