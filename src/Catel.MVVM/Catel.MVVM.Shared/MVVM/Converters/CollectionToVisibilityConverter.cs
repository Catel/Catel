// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionToVisibilityConverter.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2014 Orcomp development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN || ANDROID

namespace Catel.MVVM.Converters
{
    using System;
    using System.Collections;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Windows;
#endif

    /// <summary>
    /// Converts the count of a collection to the visibility.
    /// </summary>
    public class CollectionToCollapsingVisibilityConverter : VisibilityConverterBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionToCollapsingVisibilityConverter"/> class.
        /// </summary>
        public CollectionToCollapsingVisibilityConverter()
            : this(Visibility.Collapsed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionToCollapsingVisibilityConverter" /> class.
        /// </summary>
        /// <param name="visibility">The visibility.</param>
        public CollectionToCollapsingVisibilityConverter(Visibility visibility)
            : base(visibility)
        {
        }
        #endregion

        /// <summary>
        /// Determines whether the specified value is visible.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns><c>true</c> if the specified value is visible; otherwise, <c>false</c>.</returns>
        protected override bool IsVisible(object value, Type targetType, object parameter)
        {
            var isVisible = false;

            var collection = value as ICollection;
            if (collection != null)
            {
                isVisible = collection.Count > 0;
            }

            if (!isVisible && value is long)
            {
                isVisible = ((long)value) > 0;
            }

            if (!isVisible && value is int)
            {
                isVisible = ((int)value) > 0;
            }

            if (!isVisible && value is short)
            {
                isVisible = ((short)value) > 0;
            }

            var invertParameter = parameter as string;
            if (!string.IsNullOrWhiteSpace(invertParameter))
            {
                var invert = false;
                bool.TryParse(invertParameter, out invert);
                if (invert)
                {
                    isVisible = !isVisible;
                }
            }

            return isVisible;
        }
    }

#if NET
    /// <summary>
    /// Convert from bool to <see cref="T:System.Windows.Visibility" /> and back.
    /// The bool value true will be converted to Visibility.Visible.
    /// The bool value false will be converted to Visibility.Hidden.
    /// </summary>
    [System.Windows.Data.ValueConversion(typeof(bool), typeof(Visibility))]
    public class CollectionToHidingVisibilityConverter : CollectionToCollapsingVisibilityConverter
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CollectionToHidingVisibilityConverter()
            : base(Visibility.Hidden)
        {
        }
    }
#endif
}

#endif