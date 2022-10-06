namespace Catel.Windows.Data
{
    using System.Windows;

    /// <summary>
    /// Class to help evaluate bindings at runtime.
    /// </summary>
    internal class BindingEvaluator : FrameworkElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingEvaluator"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        public BindingEvaluator(object? dataContext = null)
        {
            DataContext = dataContext;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object? Value
        {
            get { return (object?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(BindingEvaluator), new PropertyMetadata(null));
    }
}
