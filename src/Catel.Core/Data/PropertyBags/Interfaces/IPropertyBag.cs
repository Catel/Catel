namespace Catel.Data
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Defines a property bag allowing to store values inside.
    /// </summary>
    public interface IPropertyBag : INotifyPropertyChanged
    {
        /// <summary>
        /// Sets a value inside the property bag.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="name">The name of the value.</param>
        /// <param name="value">The actual value.</param>
        void SetValue<TValue>(string name, TValue value);

        /// <summary>
        /// Gets a value inside the property bag.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="name">The name of the value.</param>
        /// <param name="defaultValue">The default value of the property.</param>
        /// <returns>The value or the specified default value in case the property is not set yet.</returns>
        TValue GetValue<TValue>(string name, TValue defaultValue = default);

        /// <summary>
        /// Determines whether the specified property is available on the property bag, which means it has a value.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns><c>true</c> if the property is available; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        public bool IsAvailable(string name);

        /// <summary>
        /// Gets all the available property names in the property bag.
        /// </summary>
        /// <returns>An array of property names.</returns>
        public string[] GetAllNames();
    }
}
