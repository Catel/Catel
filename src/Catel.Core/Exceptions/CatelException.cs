namespace Catel
{
    using System;

    public class CatelException : Exception
    {
        public CatelException()
            : base()
        {
        }

        public CatelException(string message)
            : base(message)
        {
        }

        public CatelException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
