// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FastObservableCollection.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Collections
{
    using System.Collections.Specialized;

    /// <summary>
    /// The suspension mode.
    /// </summary>
    public enum SuspensionMode
    {
        /// <summary>
        /// No specific mode. When the suspension stops, this will result in a single <see cref="NotifyCollectionChangedAction.Reset"/>
        /// event with the right added and removed items.
        /// </summary>
        None,

        /// <summary>
        /// The adding.
        /// </summary>
        Adding,

        /// <summary>
        /// The removing.
        /// </summary>
        Removing,

        /// <summary>
        /// Mixed mode (combination of Adding and Removing). This behaves the same as <see cref="None"/>, except
        /// that this raises only <see cref="NotifyCollectionChangedAction.Add"/> and <see cref="NotifyCollectionChangedAction.Remove"/>
        /// instead of <see cref="NotifyCollectionChangedAction.Reset"/> events.
        /// </summary>
        Mixed
    }
}