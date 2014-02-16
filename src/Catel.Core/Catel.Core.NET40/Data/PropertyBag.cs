// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyBag.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Class that is able to manage all properties of a specific object in a thread-safe manner.
    /// </summary>
    public class PropertyBag : INotifyPropertyChanged
    {
        #region Fields
        private readonly object _lockObject = new object();

        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBag"/> class.
        /// </summary>
        public PropertyBag()
        {
        }
        #endregion

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the property using the indexer.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The value of the property.</returns>
        public object this[string name]
        {
            get { return GetPropertyValue<object>(name); }
            set { SetPropertyValue(name, value); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Imports the properties in the existing dictionary.
        /// <para />
        /// This method will overwrite all existing property values in the property bag.
        /// </summary>
        /// <param name="propertiesToImport">The properties to import.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertiesToImport"/> is <c>null</c>.</exception>
        public void Import(Dictionary<string, object> propertiesToImport)
        {
            Argument.IsNotNull("propertiesToImport", propertiesToImport);

            foreach (var property in propertiesToImport)
            {
                SetPropertyValue(property.Key, property.Value);
            }
        }

        /// <summary>
        /// Determines whether the specified property is available on the property bag, which means it has a value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><c>true</c> if the property is available; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName" /> is <c>null</c> or whitespace.</exception>
        public bool IsPropertyAvailable(string propertyName)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            lock (_lockObject)
            {
                return _properties.ContainsKey(propertyName);
            }
        }

        /// <summary>
        /// Gets all the currently available properties in the property bag.
        /// </summary>
        /// <returns>A list of all property names and values.</returns>
        public Dictionary<string, object> GetAllProperties()
        {
            lock (_lockObject)
            {
                return _properties.ToDictionary(x => x.Key, x => x.Value);
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <typeparam name="TValue">The type of the T value.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The property value or the default value of <typeparamref name="TValue" /> if the property does not exist.</returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName" /> is <c>null</c> or whitespace.</exception>
        public TValue GetPropertyValue<TValue>(string propertyName)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            lock (_lockObject)
            {
                return _properties.ContainsKey(propertyName) ? (TValue)_properties[propertyName] : default(TValue);
            }
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">The <paramref name="propertyName" /> is <c>null</c> or whitespace.</exception>
        public void SetPropertyValue(string propertyName, object value)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            lock (_lockObject)
            {
                if (!_properties.ContainsKey(propertyName) || !ObjectHelper.AreEqualReferences(_properties[propertyName], value))
                {
                    _properties[propertyName] = value;

                    RaisePropertyChanged(propertyName);
                }
            }
        }

        /// <summary>
        /// Updates the property value by retrieving it from the property bag. After invoking the update action,
        /// the value will be written back to the property bag.
        /// </summary>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="update">The update.</param>
        public void UpdatePropertyValue<TValue>(string propertyName, Func<TValue, TValue> update)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);
            Argument.IsNotNull("update", update);

            lock (_lockObject)
            {
                if (!_properties.ContainsKey(propertyName))
                {
                    return;
                }

                var value = (TValue) _properties[propertyName];
                var updatedValue = update(value);

                SetPropertyValue(propertyName, updatedValue);
            }
        }
        #endregion
    }
}