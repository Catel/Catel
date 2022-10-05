namespace Catel.Configuration
{
    using System;

    /// <summary>
    /// The configuration changed event args class.
    /// </summary>
    public class ConfigurationChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationChangedEventArgs" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <param name="newValue">The new value.</param>
        public ConfigurationChangedEventArgs(ConfigurationContainer container, string key, object? newValue)
        {
            Container = container;
            Key = key;
            NewValue = newValue;
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public ConfigurationContainer Container { get; private set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        /// <value>The new value.</value>
        public object? NewValue { get; private set; }
    }
}
