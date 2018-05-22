// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataContextSubscriptionService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM
{
    using System;
    using System.Windows;

    /// <summary>
    /// Data context subscription mode.
    /// </summary>
    public enum DataContextSubscriptionMode
    {
        /// <summary>
        /// The direct data context only.
        /// </summary>
        DirectDataContext,

        /// <summary>
        /// Tee direct data context and the inherited data context.
        /// </summary>
        InheritedDataContext
    }

    /// <summary>
    /// Service that determines how to subscribe to a data context.
    /// </summary>
    public interface IDataContextSubscriptionService
    {
        /// <summary>
        /// Gets or sets the default data context subscription mode.
        /// </summary>
        /// <value>The default data context subscription mode.</value>
        DataContextSubscriptionMode DefaultDataContextSubscriptionMode { get; set; }

        /// <summary>
        /// Gets the data context subscription mode for the specific view.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <returns>The data context subscription mode.</returns>
        DataContextSubscriptionMode GetDataContextSubscriptionMode(Type viewType);
    }
}