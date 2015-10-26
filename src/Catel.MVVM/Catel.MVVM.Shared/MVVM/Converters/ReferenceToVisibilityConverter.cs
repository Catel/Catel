// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceToVisibilityConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN || ANDROID

namespace Catel.MVVM.Converters
{
    using System;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Windows;
#endif

    /// <summary>
    /// Convert from reference to <see cref="Visibility"/>. 
    /// If the reference contains a value, Visibility.Visible will be returned. 
    /// If the reference is null, Visibility.Collapsed will be returned.
    /// </summary>
#if NET
    [System.Windows.Data.ValueConversion(typeof (object), typeof (Visibility))]
#endif
    public class ReferenceToCollapsingVisibilityConverter : VisibilityConverterBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceToCollapsingVisibilityConverter"/> class.
        /// </summary>
        public ReferenceToCollapsingVisibilityConverter()
            : base(Visibility.Collapsed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceToCollapsingVisibilityConverter"/> class.
        /// </summary>
        /// <param name="notVisibleVisibility">The <see cref="Visibility"/> state when not visibible should be returned.</param>
        /// <exception cref="ArgumentException">The <paramref name="notVisibleVisibility"/> is <see cref="Visibility.Visible"/>.</exception>
        internal ReferenceToCollapsingVisibilityConverter(Visibility notVisibleVisibility)
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

            return invert ? (value == null) : (value != null);
        }
    }

#if NET
    /// <summary>
    /// Convert from reference to <see cref="Visibility"/>. 
    /// If the reference contains a value, Visibility.Visible will be returned. 
    /// If the reference is null, Visibility.Hidden will be returned.
    /// </summary>
    [System.Windows.Data.ValueConversion(typeof(object), typeof(Visibility))]
    public class ReferenceToHidingVisibilityConverter : ReferenceToCollapsingVisibilityConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceToHidingVisibilityConverter"/> class.
        /// </summary>
        public ReferenceToHidingVisibilityConverter()
            : base(Visibility.Hidden)
        {
        }
    }
#endif
}

#endif