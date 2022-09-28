namespace Catel.Windows
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    /// <summary>
    /// Extensions for <see cref="FrameworkElement"/>.
    /// </summary>
    public static partial class FrameworkElementExtensions
    {
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

            frameworkElement.ApplyTemplate();

            var adornerLayer = AdornerLayer.GetAdornerLayer(frameworkElement);
            if (adornerLayer is not null)
            {
                adornerLayer.Visibility = Visibility.Collapsed;
            }

            Validation.SetValidationAdornerSite(frameworkElement, null);
            Validation.SetErrorTemplate(frameworkElement, null);
        }
    }
}
