// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyStringToVisibilityConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Data.Converters
{
    using System;
    using System.Windows;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Data;
#else
    using System.Windows.Data;
#endif

    /// <summary>
    /// Convert from string to <see cref="System.Windows.Visibility"/>. 
    /// If the string is not null or empty, Visibility.Visible will be returned. 
    /// If the string is null or empty, Visibility.Collapsed will be returned.
    /// </summary>
#if NET
    [ValueConversion(typeof (string), typeof (Visibility))]
#endif
    public class EmptyStringToCollapsingVisibilityConverter : VisibilityConverterBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyStringToCollapsingVisibilityConverter"/> class.
        /// </summary>
        public EmptyStringToCollapsingVisibilityConverter()
            : base(Visibility.Collapsed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyStringToCollapsingVisibilityConverter"/> class.
        /// </summary>
        /// <param name="notVisibleVisibility">The <see cref="Visibility"/> state when not visibible should be returned.</param>
        /// <exception cref="ArgumentException">The <paramref name="notVisibleVisibility"/> is <see cref="Visibility.Visible"/>.</exception>
        internal EmptyStringToCollapsingVisibilityConverter(Visibility notVisibleVisibility)
            : base(notVisibleVisibility)
        {
        }
        #endregion

        /// <summary>
        /// Determines what value this converter should return.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is visible; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsVisible(object value, Type targetType, object parameter)
        {
            bool invert = ConverterHelper.ShouldInvert(parameter);
            string stringValue = value as string;

            if (invert)
            {
                return string.IsNullOrEmpty(stringValue);
            }

            return !string.IsNullOrEmpty(stringValue);
        }
    }

#if NET
    /// <summary>
    /// Convert from string to <see cref="System.Windows.Visibility"/>. 
    /// If the string is not null or empty, Visibility.Visible will be returned. 
    /// If the string is null or empty, Visibility.Hidden will be returned.
    /// </summary>
    [ValueConversion(typeof (string), typeof (Visibility))]
    public class EmptyStringToHidingVisibilityConverter : EmptyStringToCollapsingVisibilityConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyStringToHidingVisibilityConverter"/> class.
        /// </summary>
        public EmptyStringToHidingVisibilityConverter()
            : base(Visibility.Hidden)
        {
        }
    }
#endif
}