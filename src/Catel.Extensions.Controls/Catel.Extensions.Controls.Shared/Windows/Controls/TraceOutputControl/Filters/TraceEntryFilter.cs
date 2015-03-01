// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceEntryFilter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


#if NET

namespace Catel.Windows.Controls.Filters
{
    using System.Globalization;
    using Catel.Windows.Controls;
    using Logging;

    /// <summary>
    /// Class TraceEntryFilter.
    /// </summary>
    public class TraceEntryFilter : CompositeFilter<TraceEntry>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceEntryFilter" /> class.
        /// </summary>
        /// <param name="filterText">The filter text.</param>
        /// <param name="logEvent">The log event.</param>
        public TraceEntryFilter(string filterText, LogEvent logEvent)
        {
            if (!string.IsNullOrEmpty(filterText))
            {
                Includes += item => item.Message.ToLower(CultureInfo.InvariantCulture).Contains(filterText);
            }

            Excludes += item => item.LogEvent < logEvent;
        }
        #endregion
    }
}

#endif