// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanToVisibilityConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
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
    /// Convert from bool to <see cref="T:System.Windows.Visibility" /> and back.
    /// The bool value true will be converted to Visibility.Visible.
    /// The bool value false will be converted to Visibility.Collapsed.
    /// </summary>
#if NET
    [ValueConversion(typeof (bool), typeof (Visibility))]
#endif
    public class BooleanToCollapsingVisibilityConverter : VisibilityConverterBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanToCollapsingVisibilityConverter"/> class.
        /// </summary>
        public BooleanToCollapsingVisibilityConverter()
            : base(Visibility.Collapsed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanToCollapsingVisibilityConverter"/> class.
        /// </summary>
        /// <param name="notVisibleVisibility">The <see cref="Visibility"/> state when not visibible should be returned.</param>
        /// <exception cref="ArgumentException">The <paramref name="notVisibleVisibility"/> is <see cref="Visibility.Visible"/>.</exception>
        internal BooleanToCollapsingVisibilityConverter(Visibility notVisibleVisibility)
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
            bool? param = null;
            bool tmp;

            if (parameter != null && bool.TryParse(parameter.ToString(), out tmp))
            {
                param = tmp;
            }

            if (value is bool)
            {
                if (!param.HasValue)
                {
                    return (bool) value;
                }

                return ((bool) value == param.Value);
            }

            return false;
        }

        /// <summary>
        /// Convert Visibility back to bool.
        /// </summary>
        /// <param name="value">A value. Only value of type <see cref="T:System.Windows.Visibility" /> is supported,</param>
        /// <param name="targetType">A targettype, currently not used.</param>
        /// <param name="parameter">A parameter value, currently not used.</param>
        /// <returns>
        /// When value is Visibility.Visible then true else false.
        /// </returns>
        protected override object ConvertBack(object value, Type targetType, object parameter)
        {
            bool? param = null;
            bool tmp;

            if (parameter != null && bool.TryParse(parameter.ToString(), out tmp))
            {
                param = tmp;
            }

            if (value is Visibility)
            {
                bool isVisible = (Visibility) value == Visibility.Visible;

                if (param.HasValue)
                {
                    return isVisible == param;
                }

                return isVisible;
            }

            return false;
        }
    }

#if NET
    /// <summary>
    /// Convert from bool to <see cref="T:System.Windows.Visibility" /> and back.
    /// The bool value true will be converted to Visibility.Visible.
    /// The bool value false will be converted to Visibility.Hidden.
    /// </summary>
    [ValueConversion(typeof (bool), typeof (Visibility))]
    public class BooleanToHidingVisibilityConverter : BooleanToCollapsingVisibilityConverter
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BooleanToHidingVisibilityConverter()
            : base(Visibility.Hidden)
        {
        }
    }
#endif
}