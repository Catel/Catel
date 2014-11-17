// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UICompletedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;

    /// <summary>
    /// This is the EventArgs return value for the IUIVisualizer.Show completed event.
    /// </summary>
    public class UICompletedEventArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UICompletedEventArgs"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="result">The result.</param>
        public UICompletedEventArgs(object dataContext, bool? result)
        {
            DataContext = dataContext;
            Result = result;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the data context.
        /// </summary>
        /// <value>The data context.</value>
        public object DataContext { get; private set; }

        /// <summary>
        /// Gets the result of the window.
        /// </summary>
        /// <value>The result.</value>
        public bool? Result { get; private set; }
        #endregion
    }
}
