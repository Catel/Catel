// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyChangingEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE || PCL

namespace System.ComponentModel
{
    /// <summary>
    /// PropertyChangingEventArgs implementation for platforms not supporting INotifyPropertyChanging.
    /// </summary>
    public class PropertyChangingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangingEventArgs"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public PropertyChangingEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string PropertyName { get; private set; }
    }
}

#endif