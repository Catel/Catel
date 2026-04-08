namespace Catel.MVVM.Converters;

using System;
using Logging;
using Microsoft.Extensions.Logging;

/// <summary>
/// Debug converter that allows to debug bindings easily and writes the output to the log.
/// </summary>
public partial class DebugConverter : ValueConverterBase
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(DebugConverter));

    /// <summary>
    /// Modifies the source data before passing it to the target for display in the UI.
    /// </summary>
    /// <param name="value">The source data being passed to the target.</param>
    /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
    /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
    /// <returns>The value to be passed to the target dependency property.</returns>
    protected override object? Convert(object? value, Type targetType, object? parameter)
    {
        Logger.LogDebug("Debugging converter");
        Logger.LogDebug("  Value: {0}", ObjectToStringHelper.ToString(value));
        Logger.LogDebug("  TargetType: {0}", targetType.Name);
        Logger.LogDebug("  Parameter: {0}", ObjectToStringHelper.ToString(parameter));

        return value;
    }
}
