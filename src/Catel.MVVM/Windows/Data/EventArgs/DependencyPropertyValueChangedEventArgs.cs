namespace Catel.Windows.Data
{
    using System;
    using System.Windows;

    /// <summary>
    /// Provides data for a <see cref="DependencyPropertyChangedHelper"/> implementation.
    /// </summary>
    public class DependencyPropertyValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyPropertyValueChangedEventArgs"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="e"/> is <c>null</c>.</exception>
        internal DependencyPropertyValueChangedEventArgs(string propertyName, DependencyPropertyChangedEventArgs e)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);
            ArgumentNullException.ThrowIfNull(e);
            
            FxEventArgs = e;
            PropertyName = propertyName;
            DependencyProperty = e.Property;
            OldValue = e.OldValue;
            NewValue = e.NewValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyPropertyValueChangedEventArgs"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="dependencyProperty">Dependency property.</param>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        public DependencyPropertyValueChangedEventArgs(string propertyName, DependencyProperty dependencyProperty, object oldValue, object newValue)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);
            Argument.IsNotNull("dependencyProperty", dependencyProperty);

            FxEventArgs = new DependencyPropertyChangedEventArgs(dependencyProperty, oldValue, newValue);
            PropertyName = propertyName;
            DependencyProperty = dependencyProperty;
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Gets the framework event args.
        /// </summary>
        /// <value>The framework event args.</value>
        public DependencyPropertyChangedEventArgs FxEventArgs { get; private set; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <remarks></remarks>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets the dependency property that has changed.
        /// </summary>
        public DependencyProperty DependencyProperty { get; private set; }

        /// <summary>
        ///  Gets the value of the property before the change.
        /// </summary>
        public object OldValue { get; private set; }

        /// <summary>
        /// Gets the value of the property after the change.
        /// </summary>
        public object NewValue { get; private set; }
    }
}
