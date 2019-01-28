// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Windows.Data
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
        public static object GetBindingValue(FrameworkElement frameworkElement, BindingBase binding)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);
            Argument.IsNotNull("binding", binding);

            var evaluator = new BindingEvaluator(frameworkElement.DataContext);
            BindingOperations.SetBinding(evaluator, BindingEvaluator.ValueProperty, binding);
            object value = evaluator.Value;
            ClearBinding(evaluator, BindingEvaluator.ValueProperty);

            return value;
        }

        /// <summary>
        /// Clears a binding. This method implements the ClearBinding for both WPF and Silverlight.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        public static void ClearBinding(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
#if NET || NETCORE
            BindingOperations.ClearBinding(dependencyObject, dependencyProperty);
#else
            // Other platforms do not support ClearBinding, then we use ClearValue
            dependencyObject.ClearValue(dependencyProperty);
#endif
        }
    }
}

#endif
