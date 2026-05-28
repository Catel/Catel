namespace Catel.Windows.Controls;

using System;
using System.Windows;
using MVVM.Converters;

/// <summary>
/// Converter for the <see cref="InfoBarMessageControl"/> to determine whether the control
/// should be visible for the current mode and
/// </summary>
[System.Windows.Data.ValueConversion(typeof(InfoBarMessageControlMode), typeof(object), ParameterType = typeof(InfoBarMessageControlMode))]
public class InfoBarMessageControlVisibilityConverter : VisibilityConverterBase
{
    public InfoBarMessageControlVisibilityConverter()
    {
    }

    protected override bool IsVisible(object? value, Type targetType, object? parameter)
    {
        if (value is null)
        {
            return false;
        }

        var mode = (parameter is InfoBarMessageControlMode) ? (InfoBarMessageControlMode)parameter : InfoBarMessageControlMode.Inline;

        if (parameter is string)
        {
            if (Enum<InfoBarMessageControlMode>.TryParse((string)parameter, out var parsedMode))
            {
                mode = parsedMode;
            }
        }

        return (InfoBarMessageControlMode)value == mode;
    }
}
