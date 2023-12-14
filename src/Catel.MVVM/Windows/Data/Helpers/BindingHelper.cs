namespace Catel.Windows.Data
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Binding helper class.
    /// </summary>
    public static class BindingHelper
    {
        /// <summary>
        /// Gets the binding value.
        /// </summary>
        /// <param name="frameworkElement">The dependency object.</param>
        /// <param name="binding">The binding.</param>
        /// <returns>The actual binding value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="binding"/> is <c>null</c>.</exception>
        public static object? GetBindingValue(FrameworkElement frameworkElement, BindingBase binding)
        {
            ArgumentNullException.ThrowIfNull(frameworkElement);
            ArgumentNullException.ThrowIfNull(binding);

            var evaluator = new BindingEvaluator(frameworkElement.DataContext);
            BindingOperations.SetBinding(evaluator, BindingEvaluator.ValueProperty, binding);
            var value = evaluator.Value;
            ClearBinding(evaluator, BindingEvaluator.ValueProperty);

            return value;
        }

        /// <summary>
        /// Clears a binding. This method implements the ClearBinding for all xaml platforms.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        public static void ClearBinding(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            BindingOperations.ClearBinding(dependencyObject, dependencyProperty);
        }
    }
}
