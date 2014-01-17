// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
        /// Determines whether the specified data context is a sentinel.
        /// <para />
        /// For more information, see http://stackoverflow.com/questions/3868786/wpf-sentinel-objects-and-how-to-check-for-an-internal-type.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <returns><c>true</c> if the data context is a sentinel; otherwise, <c>false</c>.</returns>
        public static bool IsSentinelObject(object dataContext)
        {
            if (dataContext == null)
            {
                return false;
            }

            var type = dataContext.GetType();
            if (string.CompareOrdinal(type.FullName, "MS.Internal.NamedObject") == 0)
            {
                return true;
            }
            
            if (string.CompareOrdinal(dataContext.ToString(), "{DisconnectedObject}") == 0)
            {
                return true;
            }

            return false;
        }

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
#if NET
            BindingOperations.ClearBinding(dependencyObject, dependencyProperty);
#else
            // Other platforms do not support ClearBinding, then we use ClearValue
            dependencyObject.ClearValue(dependencyProperty);
#endif
        }
    }
}
