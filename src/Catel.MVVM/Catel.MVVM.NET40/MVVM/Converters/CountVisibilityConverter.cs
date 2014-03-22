// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CountVisibilityConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Converters
{
    using System;
    using System.Collections;
    using System.Windows;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Data;
#else
    using System.Windows.Data;
#endif

    /// <summary>
    /// Convert for auto collapsing of control depending on given count.
    /// </summary>
#if NET
    [ValueConversion(typeof (int), typeof (Visibility))]
#endif
    public class CountCollapsedConverter : VisibilityConverterBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CountCollapsedConverter"/> class.
        /// </summary>
        public CountCollapsedConverter()
            : base(Visibility.Collapsed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountCollapsedConverter"/> class.
        /// </summary>
        /// <param name="notVisibleVisibility">The <see cref="Visibility"/> state when not visibible should be returned.</param>
        /// <exception cref="ArgumentException">The <paramref name="notVisibleVisibility"/> is <see cref="Visibility.Visible"/>.</exception>
        internal CountCollapsedConverter(Visibility notVisibleVisibility)
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
        /// <c>true</c> if the specified value is visible; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsVisible(object value, Type targetType, object parameter)
        {
            if (value == null)
            {
                return false;
            }

            bool hasContent = false;

            if (!hasContent && value is ICollection)
            {
                hasContent = ((ICollection) value).Count > 0;
            }

            if (!hasContent && value is string)
            {
                hasContent = ((string) value).Length > 0;
            }

            if (!hasContent && value is long)
            {
                hasContent = ((long) value) > 0;
            }

            if (!hasContent && value is int)
            {
                hasContent = ((int) value) > 0;
            }

            if (!hasContent && value is short)
            {
                hasContent = ((short) value) > 0;
            }

            return hasContent;
        }
    }

#if NET
    /// <summary>
    /// Convert for auto hiding of control depending on given count.
    /// </summary>
    [ValueConversion(typeof (int), typeof (Visibility))]
    public class CountHiddenConverter : CountCollapsedConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CountHiddenConverter"/> class.
        /// </summary>
        public CountHiddenConverter()
            : base(Visibility.Hidden)
        {
        }
    }
#endif
}