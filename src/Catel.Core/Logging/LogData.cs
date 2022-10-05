namespace Catel.Logging
{
    using System.Collections.Generic;

    /// <summary>
    /// Class containing log data.
    /// </summary>
    public class LogData : Dictionary<string, object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogData"/> class.
        /// </summary>
        public LogData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogData"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public LogData(IDictionary<string, object> values)
            : base(values)
        {
        }
    }
}
