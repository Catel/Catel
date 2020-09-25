// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyBag.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class that is able to manage all properties of a specific object in a thread-safe manner.
    /// </summary>
    public partial class PropertyBag : PropertyBagBase
    {
        #region Fields
        private readonly object _lockObject = new object();

        private readonly IDictionary<string, object> _properties;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBag"/> class.
        /// </summary>
        public PropertyBag()
            : this(new Dictionary<string, object>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBag"/> class.
        /// </summary>
        /// <param name="propertyDictionary">The property dictionary.</param>
        public PropertyBag(IDictionary<string, object> propertyDictionary)
        {
            Argument.IsNotNull(nameof(propertyDictionary), propertyDictionary);

            _properties = propertyDictionary;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the property using the indexer.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The value of the property.</returns>
        public object this[string name]
        {
            get { return GetValue<object>(name); }
            set { SetValue(name, value); }
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
                SetValue(property.Key, property.Value);
            }
        }

        public override bool IsAvailable(string name)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            lock (_lockObject)
            {
                return _properties.ContainsKey(name);
            }
        }

        /// <summary>
        /// Gets all the currently available properties in the property bag.
        /// </summary>
        /// <returns>A list of all property names and values.</returns>
        public override Dictionary<string, object> GetAllProperties()
        {
            lock (_lockObject)
            {
                return _properties.ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public override string[] GetAllNames()
        {
            lock (_lockObject)
            {
                return _properties.Keys.ToArray();
            }
        }

        public override TValue GetValue<TValue>(string name, TValue defaultValue = default)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            lock (_lockObject)
            {
                if (_properties.TryGetValue(name, out var propertyValue))
                {
                    return (TValue)propertyValue;

                    //// Safe-guard null values for value types
                    //if (!(propertyValue is null) || typeof(TValue).IsNullableType())
                    //{
                    //    return (TValue)propertyValue;
                    //}
                }

                return defaultValue;
            }
        }

        public override void SetValue<TValue>(string name, TValue value)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            var raisePropertyChanged = false;

            lock (_lockObject)
            {
                if (!_properties.TryGetValue(name, out var propertyValue) || !ObjectHelper.AreEqualReferences(propertyValue, BoxingCache.GetBoxedValue(value)))
                {
                    _properties[name] = value;
                    raisePropertyChanged = true;
                }
            }

            if (raisePropertyChanged)
            {
                RaisePropertyChanged(name);
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
                if (!_properties.TryGetValue(propertyName, out var propertyValue))
                {
                    return;
                }

                var value = (TValue) propertyValue;
                var updatedValue = update(value);

                SetValue(propertyName, updatedValue);
            }
        }
        #endregion
    }
}
