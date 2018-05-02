// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DetermineViewModelInstanceEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Providers
{
    using System;
    using MVVM;

    /// <summary>
    /// EventArgs class which allows late-time dynamic view model determination.
    /// </summary>
    public class DetermineViewModelInstanceEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetermineViewModelInstanceEventArgs"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        public DetermineViewModelInstanceEventArgs(object dataContext)
        {
            DataContext = dataContext;
        }

        /// <summary>
        /// Gets the data context.
        /// </summary>
        public object DataContext { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the logic should create a view model by itself when the
        /// <see cref="ViewModel"/> is <c>null</c>.
        /// <para />
        /// By default, this value is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if the logic should not create a view model by itself; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool DoNotCreateViewModel { get; set; }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        /// <value>The type of the view model.</value>
        public IViewModel ViewModel { get; set; }
    }
}