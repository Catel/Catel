namespace Catel.Windows.Controls
{
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
            : base(Visibility.Collapsed)
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
                Enum<InfoBarMessageControlMode>.TryParse((string)parameter, out mode);
            }

            return (InfoBarMessageControlMode)value == mode;
        }
    }
}
