// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewPropertySelector.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Views
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Selector class to keep the view property selections to a minimum. Catel uses a special wrapping
    /// technology to wrap bindings to view properties to be able to add change notifications for all target
    /// platforms.
    /// <para />
    /// Though this technology works great, it might have impact on performance and this is not always necessary. By
    /// customizing the <see cref="IViewPropertySelector"/>, developers can tweak the interesting view properties
    /// per type.
    /// </summary>
    public class ViewPropertySelector : IViewPropertySelector
    {
        /// <summary>
        /// Determines whether all view properties must be subscribed for this type.
        /// </summary>
        /// <param name="targetViewType">Type of the target view.</param>
        /// <returns><c>true</c> if all view properties must be subscribed to, <c>false</c> otherwise.</returns>
        public virtual bool MustSubscribeToAllViewProperties(Type targetViewType)
        {
            return true;
        }

        /// <summary>
        /// Gets the view properties to subscribe to for the specified target view type. 
        /// <para />
        /// If the <see cref="MustSubscribeToAllViewProperties"/> returns <c>true</c> for the specified target view
        /// type, this method will not be called and can return an empty list.
        /// </summary>
        /// <param name="targetViewType">Type of the target view.</param>
        /// <returns>The list of view properties to subscribe to.</returns>
        public virtual List<string> GetViewPropertiesToSubscribeTo(Type targetViewType)
        {
            return new List<string>();
        }
    }

    /// <summary>
    /// Very fast view property selector because it does not select any view properties.
    /// <para />
    /// Use this one for best performance but loose the automatic view property change notifications.
    /// </summary>
    public class FastDependencyPropertySelector : ViewPropertySelector
    {
        /// <summary>
        /// Determines whether all view properties must be subscribed for this type.
        /// </summary>
        /// <param name="targetViewType">Type of the target view.</param>
        /// <returns><c>true</c> if all view properties must be subscribed to, <c>false</c> otherwise.</returns>
        public override bool MustSubscribeToAllViewProperties(Type targetViewType)
        {
            return false;
        }
    }
}