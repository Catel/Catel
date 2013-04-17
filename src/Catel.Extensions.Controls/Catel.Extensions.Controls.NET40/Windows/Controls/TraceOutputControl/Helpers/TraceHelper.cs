// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Diagnostics
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Trace helper class.
    /// </summary>
    public static class TraceHelper
    {
        #region Variables
        /// <summary>
        /// Available trace levels.
        /// </summary>
        private static Dictionary<TraceEventType, TraceLevel> _traceLevels;
        #endregion

        #region Methods
        /// <summary>
        /// Converts a <see cref="TraceEventType"/> to a <see cref="TraceLevel"/>.
        /// </summary>
        /// <param name="eventType"><see cref="TraceEventType"/> to convert.</param>
        /// <returns><see cref="TraceLevel"/> that represents a <see cref="TraceEventType"/>.</returns>
        public static TraceLevel ConvertTraceEventTypeToTraceLevel(TraceEventType eventType)
        {
            if (_traceLevels == null)
            {
                _traceLevels = new Dictionary<TraceEventType, TraceLevel>();
                _traceLevels.Add(TraceEventType.Critical, TraceLevel.Error);
                _traceLevels.Add(TraceEventType.Error, TraceLevel.Error);
                _traceLevels.Add(TraceEventType.Warning, TraceLevel.Warning);
                _traceLevels.Add(TraceEventType.Information, TraceLevel.Info);
                _traceLevels.Add(TraceEventType.Verbose, TraceLevel.Verbose);
            }

            return _traceLevels.ContainsKey(eventType) ? _traceLevels[eventType] : TraceLevel.Off;
        }
        #endregion
    }
}