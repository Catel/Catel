// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdvancedPropertyChangingEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System.ComponentModel;

    /// <summary>
    /// Class with additional control on the <see cref="INotifyPropertyChanging"/> interface.
    /// </summary>
    public class AdvancedPropertyChangingEventArgs : PropertyChangingEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ComponentModel.PropertyChangingEventArgs" /> class.
        /// </summary>
        /// <param name="propertyName">The name of the property whose value is changing.</param>
        public AdvancedPropertyChangingEventArgs(string propertyName)
            : base(propertyName)
        {
            Cancel = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the setter action should be canceled.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel { get; set; }
    }
}