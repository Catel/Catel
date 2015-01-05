// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IView.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Views
{
    using System;

    /// <summary>
    /// Interface defining the base functionality of a view required to interact with Catel.
    /// </summary>
    public interface IView : IViewModelContainer
    {
        #region Properties
        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>
        /// The data context.
        /// </value>
        object DataContext { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        object Tag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the view is enabled.
        /// </summary>
        /// <value><c>true</c> if the view is enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the view is loaded.
        /// </summary>
        event EventHandler<EventArgs> Loaded;

        /// <summary>
        /// Occurs when the view is unloaded.
        /// </summary>
        event EventHandler<EventArgs> Unloaded;

        /// <summary>
        /// Occurs when the data context has changed.
        /// </summary>
        event EventHandler<DataContextChangedEventArgs> DataContextChanged;
        #endregion
    }
}