// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DetermineViewModelTypeEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls.MVVMProviders.Logic
{
    using System;

    /// <summary>
    /// EventArgs class which allows late-time dynamic view model determination.
    /// </summary>
    public class DetermineViewModelTypeEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetermineViewModelTypeEventArgs"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        public DetermineViewModelTypeEventArgs(object dataContext)
        {
            DataContext = dataContext;
        }

        /// <summary>
        /// Gets the data context.
        /// </summary>
        public object DataContext { get; private set; }

        /// <summary>
        /// Gets or sets the type of the view model.
        /// </summary>
        /// <value>The type of the view model.</value>
        public Type ViewModelType { get; set; }
    }
}