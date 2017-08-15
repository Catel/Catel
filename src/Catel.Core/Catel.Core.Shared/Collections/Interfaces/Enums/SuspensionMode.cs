// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuspensionMode.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
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
        Mixed,

        /// <summary>
        /// MixedBash mode (combination of Adding and Removing). This behaves the same as <see cref="Mixed"/>, except
        /// that this raises multiply <see cref="NotifyCollectionChangedAction.Add"/> events and <see cref="NotifyCollectionChangedAction.Remove"/>
        /// events instead of single <see cref="NotifyCollectionChangedAction.Reset"/> event.
        /// </summary>
        MixedBash
    }
}