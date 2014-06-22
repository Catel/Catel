// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionToVisibilityConverter.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2014 Orcomp development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN

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
    public class CollectionToVisibilityConverter : VisibilityConverterBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionToVisibilityConverter"/> class.
        /// </summary>
        public CollectionToVisibilityConverter()
            : base(Visibility.Collapsed)
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
            bool isVisible = false;

            var collection = value as ICollection;
            if (collection != null)
            {
                isVisible = collection.Count > 0;
            }

            var invertParameter = parameter as string;
            if (!string.IsNullOrWhiteSpace(invertParameter))
            {
                bool invert = false;
                bool.TryParse(invertParameter, out invert);
                if (invert)
                {
                    isVisible = !isVisible;
                }
            }

            return isVisible;
        }
    }
}

#endif