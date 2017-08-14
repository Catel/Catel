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
        /// event.
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
        /// Mixed mode (combination of Adding and Removing). This behaves the same as <see cref="None"/>, except that it holds additional
        /// lists of the changed items, their indices and the concrete actions.
        /// </summary>
        Mixed
    }
}