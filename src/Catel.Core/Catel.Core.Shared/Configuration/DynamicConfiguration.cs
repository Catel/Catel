// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicConfiguration.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Configuration
{
    using Data;
    using Runtime.Serialization;

    /// <summary>
    /// Dynamic configuration.
    /// </summary>
    [SerializerModifier(typeof(DynamicConfigurationSerializerModifier))]
    public class DynamicConfiguration : ModelBase
    {
        #region Methods
        /// <summary>
        /// Registers the configuration key.
        /// </summary>
        /// <param name="name">The name.</param>
        public void RegisterConfigurationKey(string name)
        {
            if (IsConfigurationKeyAvailable(name))
            {
                return;
            }

            var propertyData = RegisterProperty(name, typeof (string));

            InitializePropertyAfterConstruction(propertyData);
        }

        /// <summary>
        /// Determines whether the specified configuration key is available.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified configuration key is available; otherwise, <c>false</c>.</returns>
        public bool IsConfigurationKeyAvailable(string name)
        {
            return IsPropertyRegistered(GetType(), name);
        }

        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public string GetConfigurationValue(string name)
        {
            RegisterConfigurationKey(name);

            return GetValue<string>(name);
        }

        /// <summary>
        /// Sets the configuration value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void SetConfigurationValue(string name, string value)
        {
            RegisterConfigurationKey(name);

            SetValue(name, value);
        }
        #endregion
    }
}