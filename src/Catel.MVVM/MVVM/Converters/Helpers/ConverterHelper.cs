namespace Catel.MVVM.Converters
{
    using Reflection;
    using System.Windows;

    /// <summary>
    /// Converter helper class.
    /// </summary>
    public static class ConverterHelper
    {
        /// <summary>
        /// The generic <c>UnSet</c> value, compatible with all platforms.
        /// </summary>
        public static readonly object UnsetValue =  DependencyProperty.UnsetValue;

        /// <summary>
        /// Checks whether the converted must be inverted. This checks the parameter input and checks whether
        /// it is a boolean.
        /// </summary>
        /// <param name="parameter">The parameter to check. Can be <c>null</c>.</param>
        /// <returns><c>true</c> if the converter should be inverted; otherwise <c>false</c>.</returns>
        public static bool ShouldInvert(object parameter)
        {
            var invert = false;

            if (parameter is not null)
            {
                if (TypeHelper.TryCast<bool, object>(parameter, out var shouldInvert))
                {
                    invert = shouldInvert;
                }
            }

            return invert;
        }
    }
}
