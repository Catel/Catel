﻿namespace Catel.MVVM.Converters
{
    using System;
    using Catel.Services;
    using Logging;

    /// <summary>
    /// Available modes for the <see cref="BooleanToTextConverter"/>.
    /// </summary>
    internal enum BooleanToTextConverterMode
    {
        /// <summary>
        /// True becomes <c>Yes</c>, false becomes <c>No</c>.
        /// </summary>
        YesNo,

        /// <summary>
        /// True becomes <c>x</c>, false becomes <c></c> (thus empty).
        /// </summary>
        X
    }

    /// <summary>
    /// BooleanToTextConverter.
    /// </summary>
    [System.Windows.Data.ValueConversion(typeof(bool), typeof(string))]
    public class BooleanToTextConverter : ValueConverterBase
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ILanguageService _languageService;

        public BooleanToTextConverter(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        protected override object? Convert(object? value, Type targetType, object? parameter)
        {
            if ((value is null) || !(value is bool))
            {
                return ConverterHelper.UnsetValue;
            }

            var typedValue = (bool)value;
            var mode = (parameter is BooleanToTextConverterMode) ? (BooleanToTextConverterMode)parameter : ParseMode(parameter as string);

            // Now convert the value
            switch (mode)
            {
                case BooleanToTextConverterMode.X:
                    return (typedValue) ? "x" : string.Empty;

                case BooleanToTextConverterMode.YesNo:
                    return (typedValue) ? _languageService.GetString("Yes") : _languageService.GetString("No");

                default:
                    // Some strange way, this method fails (all known modes must be handled), so return "failed"
                    return _languageService.GetString("Failed");
            }
        }

        /// <summary>
        /// Parses the parameter and returns a <see cref="BooleanToTextConverterMode"/> that represents the parameter. 
        /// </summary>
        /// <param name="parameter"><see cref="BooleanToTextConverterMode"/> as text.</param>
        /// <returns><see cref="BooleanToTextConverterMode"/> as it was passed as a string.</returns>
        /// <remarks>
        /// If the parameter is invalid, or the method fails to parse the parameter, <see cref="BooleanToTextConverterMode.YesNo"/>
        /// will be returned as a default value.
        /// </remarks>
        private static BooleanToTextConverterMode ParseMode(string? parameter)
        {
            var mode = BooleanToTextConverterMode.YesNo;

            if (string.IsNullOrEmpty(parameter))
            {
                Log.Error($"Converter parameter cannot be null, default value '{Enum<BooleanToTextConverterMode>.ToString(mode)}' will be used");
                return mode;
            }

            try
            {
                mode = (BooleanToTextConverterMode)Enum.Parse(typeof(BooleanToTextConverterMode), parameter, false);
            }
            catch (ArgumentException)
            {
                Log.Error("Failed to parse '{0}' as '{1}'", parameter, "BooleanToTextConverterMode");
                return mode;
            }

            return mode;
        }
    }
}
