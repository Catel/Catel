// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicConfigurationExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
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
        /// <returns>System.String.</returns>
        public static TValue GetConfigurationValue<TValue>(this DynamicConfiguration dynamicConfiguration, string name)
        {
            var value = (TValue)dynamicConfiguration.GetConfigurationValue(name);
            return value;
        }
    }
}