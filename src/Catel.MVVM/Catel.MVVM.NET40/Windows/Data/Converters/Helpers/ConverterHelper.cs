// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConverterHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Data.Converters
{
    using Reflection;

#if NET
    using System.Windows.Data;
#elif NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Windows;
#endif

    /// <summary>
    /// Converter helper class.
    /// </summary>
    public static class ConverterHelper
    {
        /// <summary>
        /// The generic <c>Unset</c> value, compatible with WPF and Silverlight.
        /// </summary>
        public static readonly object UnsetBindingValue = DependencyProperty.UnsetValue;

        /// <summary>
        /// Checks whether the converted must be inverted. This checks the parameter input and checks whether
        /// it is a boolean.
        /// </summary>
        /// <param name="parameter">The parameter to check. Can be <c>null</c>.</param>
        /// <returns><c>true</c> if the converter should be inverted; otherwise <c>false</c>.</returns>
        public static bool ShouldInvert(object parameter)
        {
            bool invert = false;
            if (parameter != null)
            {
                invert = TypeHelper.Cast<bool>(parameter);
            }

            return invert;
        }
    }
}