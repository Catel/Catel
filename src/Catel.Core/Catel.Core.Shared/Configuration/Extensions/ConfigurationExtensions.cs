// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Configuration
{
    using System;

    /// <summary>
    /// The configuration extension methods.
    /// </summary>
    public static partial class ConfigurationExtensions
    {
        /// <summary>
        /// Determines whether the specified <see cref="ConfigurationChangedEventArgs"/> represents the expected key.
        /// <para />
        /// A key is also expected if the key is <c>null</c> or whitespace because it represents a full scope update in the 
        /// <see cref="IConfigurationService"/>.
        /// </summary>
        /// <param name="eventArgs">The <see cref="ConfigurationChangedEventArgs"/> instance containing the event data.</param>
        /// <param name="expectedKey">The expected key.</param>
        /// <returns><c>true</c> if the event args represent the expected key; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="eventArgs"/> is <c>null</c>.</exception>
        public static bool IsConfigurationKey(this ConfigurationChangedEventArgs eventArgs, string expectedKey)
        {
            Argument.IsNotNull("eventArgs", eventArgs);

            return IsConfigurationKey(eventArgs.Key, expectedKey);
        }

        /// <summary>
        /// Determines whether the specified configuration key represents the expected key.
        /// <para />
        /// A key is also expected if the key is <c>null</c> or whitespace because it represents a full scope update in the 
        /// <see cref="IConfigurationService"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="expectedKey">The expected key.</param>
        /// <returns><c>true</c> if [is configuration key] [the specified expected key]; otherwise, <c>false</c>.</returns>
        public static bool IsConfigurationKey(this string key, string expectedKey)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return true;
            }

            return key.EqualsIgnoreCase(expectedKey);
        }
    }
}