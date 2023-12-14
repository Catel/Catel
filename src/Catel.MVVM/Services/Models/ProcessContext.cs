namespace Catel.Services
{
    public class ProcessContext
    {
        public ProcessContext()
        {
            FileName = string.Empty;
            Arguments = string.Empty;
            WorkingDirectory = string.Empty;
            Verb = string.Empty;
        }

        public string FileName { get; set; }

        public string Arguments { get; set; }

        public string WorkingDirectory { get; set; }

        public string Verb { get; set; }

        public bool UseShellExecute { get; set; }
    }
}
