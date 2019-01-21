// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewPropertySelector.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Views
{
    using Catel.Logging;
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
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly HashSet<string> _allViewsProperties = new HashSet<string>();
        private readonly Dictionary<Type, HashSet<string>> _viewProperties = new Dictionary<Type, HashSet<string>>();

        /// <summary>
        /// Adds the property to subscribe to.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="targetViewType">Type of the target view. If <c>null</c>, all target views will subscribe to this property.</param>
        public void AddPropertyToSubscribe(string propertyName, Type targetViewType)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return;
            }

            if (targetViewType is null)
            {
                Log.Debug("Added property '{0}' on all views to subscribe to", propertyName);

                if (!_allViewsProperties.Contains(propertyName))
                {
                    _allViewsProperties.Add(propertyName);
                }
            }
            else
            {
                Log.Debug("Added property '{0}.{1}' to subscribe to", targetViewType.Name, propertyName);

                if (!_viewProperties.TryGetValue(targetViewType, out var properties))
                {
                    properties = new HashSet<string>();
                    _viewProperties[targetViewType] = properties;
                }

                properties.Add(propertyName);
            }
        }

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
            var properties = new List<string>();

            properties.AddRange(_allViewsProperties);

            if (_viewProperties.TryGetValue(targetViewType, out var viewProperties))
            {
                foreach (var property in viewProperties)
                {
                    if (!properties.Contains(property))
                    {
                        properties.Add(property);
                    }
                }
            }

            return properties;
        }
    }

    /// <summary>
    /// Very fast view property selector because it does not select any view properties.
    /// <para />
    /// Use this one for best performance but loose the automatic view property change notifications.
    /// </summary>
    public class FastViewPropertySelector : ViewPropertySelector
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
