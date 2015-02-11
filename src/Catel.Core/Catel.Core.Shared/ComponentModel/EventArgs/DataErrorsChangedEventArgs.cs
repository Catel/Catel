﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataErrorsChangedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET40 || PCL

namespace System.ComponentModel
{
    /// <summary>
    /// EventArgs for the <see cref="INotifyDataErrorInfo.ErrorsChanged"/> and <see cref="INotifyDataWarningInfo.WarningsChanged"/> events.
    /// </summary>
    public class DataErrorsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataErrorsChangedEventArgs"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public DataErrorsChangedEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; private set; }
    }
}

#endif