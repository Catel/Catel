// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Data.Converters
{
    using System;
    using Logging;

    /// <summary>
    /// Debug converter that allows to debug bindings easily and writes the output to the log.
    /// </summary>
    public class DebugConverter : ValueConverterBase
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        protected override object Convert(object value, Type targetType, object parameter)
        {
            Log.Debug("Debugging converter");
            Log.Indent();
            Log.Debug("Value: {0}", ObjectToStringHelper.ToString(value));
            Log.Debug("TargetType: {0}", targetType.Name);
            Log.Debug("Parameter: {0}", ObjectToStringHelper.ToString(parameter));
            Log.Unindent();

            return value;
        }
    }
}