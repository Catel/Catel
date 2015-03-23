// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogData.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Logging
{
    using System.Collections.Generic;

    /// <summary>
    /// Class containing log data.
    /// </summary>
    public class LogData : Dictionary<string, object>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LogData"/> class.
        /// </summary>
        public LogData()
            : this(null)
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
        #endregion
    }
}