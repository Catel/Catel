// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingEvaluator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Data
{
#if NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Windows;
#endif

    /// <summary>
    /// Class to help evaluate bindings at runtime.
    /// </summary>
    internal class BindingEvaluator : FrameworkElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingEvaluator"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        public BindingEvaluator(object dataContext = null)
        {
            DataContext = dataContext;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(BindingEvaluator), new PropertyMetadata(null));
    }
}
