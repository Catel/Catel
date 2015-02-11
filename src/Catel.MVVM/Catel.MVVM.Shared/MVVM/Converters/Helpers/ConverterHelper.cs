﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConverterHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Converters
{
    using Reflection;

#if XAMARIN
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
        /// The generic <c>UnSet</c> value, compatible with WPF and Silverlight.
        /// </summary>
        public static readonly object UnsetValue = 
#if XAMARIN
            new UnsetBindingValue();
#else
            DependencyProperty.UnsetValue;
#endif

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

#if XAMARIN
    /// <summary>
    /// Unset binding value class.
    /// </summary>
    public class UnsetBindingValue
    {
    }
#endif
}