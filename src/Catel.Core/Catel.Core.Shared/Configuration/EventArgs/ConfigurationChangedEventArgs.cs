// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationChangedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Configuration
{
    using System;

    /// <summary>
    /// The configuration changed event args class.
    /// </summary>
    public class ConfigurationChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationChangedEventArgs"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="newValue">The new value.</param>
        public ConfigurationChangedEventArgs(string key, object newValue)
        {
            Key = key;
            NewValue = newValue;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        /// <value>The new value.</value>
        public object NewValue { get; private set; }
    }
}