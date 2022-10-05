namespace Catel.MVVM.Converters
{
    using System;
    using System.Windows.Data;
    using System.Windows.Markup;
    using Catel.Data;

    /// <summary>
    /// Converts a comparison of 2 bindings to a boolean whether the 
    /// objects are equal or not.
    /// </summary>
    [ValueConversion(typeof(object), typeof(object))]
    public class AreEqualMultiValueConverter : MarkupExtension, IMultiValueConverter
    {
        /// <summary>
        /// Converts the comparison of 2 values to a boolean.
        /// </summary>
        /// <param name="values">Values to convert. Only 2 values are supported.</param>
        /// <param name="targetType">Not supported.</param>
        /// <param name="parameter">Not supported.</param>
        /// <param name="culture">Not supported.</param>
        /// <returns>True if the values are equal, otherwise false.</returns>
        public object Convert(object?[] values, Type targetType, object? parameter, System.Globalization.CultureInfo? culture)
        {
            if (values.Length != 2)
            {
                return BoxingCache.GetBoxedValue(false);
            }

            return BoxingCache.GetBoxedValue(object.Equals(values[0], values[1]));
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="value">Not supported.</param>
        /// <param name="targetTypes">Not supported.</param>
        /// <param name="parameter">Not supported.</param>
        /// <param name="culture">Not supported.</param>
        /// <returns>Not supported.</returns>
        public object[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, System.Globalization.CultureInfo? culture)
        {
            // Not supported (and IMultiValueConverter must return null if no conversion is supported)
            return null;
        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
