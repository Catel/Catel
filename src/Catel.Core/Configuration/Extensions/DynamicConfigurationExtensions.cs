// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicConfigurationExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Configuration
{
    /// <summary>
    /// Extension methods for dynamic configuration.
    /// </summary>
    public static class DynamicConfigurationExtensions
    {
        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dynamicConfiguration">The dynamic configuration.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value if the configuration value is not of type TValue.</param>
        /// <returns>System.String.</returns>
        public static TValue GetConfigurationValue<TValue>(this DynamicConfiguration dynamicConfiguration, string name, TValue defaultValue)
        {
            var value = dynamicConfiguration.GetConfigurationValue(name);

            if (value is TValue)
            {
                return (TValue) value;
            }

            return defaultValue;
        }
    }
}