namespace Catel.MVVM.Converters
{
    using System;

    /// <summary>
    /// Converter for formatting values.
    /// </summary>
    [System.Windows.Data.ValueConversion(typeof(object), typeof(string))]
    public class FormattingConverter : ValueConverterBase
    {
        #region Fields
        private readonly string _defaultFormatString;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FormattingConverter"/> class.
        /// </summary>
        public FormattingConverter()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormattingConverter"/> class.
        /// </summary>
        /// <param name="defaultFormatString">A default format string.</param>
        protected FormattingConverter(string defaultFormatString)
        {
            _defaultFormatString = defaultFormatString;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var formattingToUse = (parameter as string) ?? _defaultFormatString ?? string.Empty;
            var formatString = string.Format("{{0:{0}}}", formattingToUse);

            if (formatString.Contains(" "))
            {
                formatString = formattingToUse;
            }

            var finalValue = string.Format(CurrentCulture, formatString, value);
            return finalValue;
        }
        #endregion
    }
}
