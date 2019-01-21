// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameworkElementExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Windows
{
    using System.Windows;

#if UWP
    using global::Windows.UI;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Media;

    using UIEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
    using VisualStateGroup = global::Windows.UI.Xaml.VisualStateGroup;
#else
    using System.Windows.Controls;
    using VisualStateGroup = System.Object;
#endif

#if NET || NETCORE
    using System.Windows.Documents;
#endif

    /// <summary>
    /// Extensions for <see cref="FrameworkElement"/>.
    /// </summary>
    public static partial class FrameworkElementExtensions
    {
        #region Methods
        /// <summary>
        /// Hides the validation adorner.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        public static void HideValidationAdorner(this FrameworkElement frameworkElement)
        {
            if (frameworkElement is null)
            {
                return;
            }

#if NET || NETCORE
            frameworkElement.ApplyTemplate();

            var adornerLayer = AdornerLayer.GetAdornerLayer(frameworkElement);
            if (adornerLayer != null)
            {
                adornerLayer.Visibility = Visibility.Collapsed;
            }

            Validation.SetValidationAdornerSite(frameworkElement, null);
            Validation.SetErrorTemplate(frameworkElement, null);
#endif
        }
        #endregion
    }
}

#endif
