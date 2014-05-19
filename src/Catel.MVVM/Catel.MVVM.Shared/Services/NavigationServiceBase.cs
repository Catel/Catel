// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;

    /// <summary>
    /// Abstract base class to support abstract partial methods.
    /// </summary>
    public abstract class NavigationServiceBase : ViewModelServiceBase
    {
        #region Properties
        /// <summary>
        /// Gets a value indicating whether it is possible to navigate back.
        /// </summary>
        /// <value>
        /// <c>true</c> if it is possible to navigate back; otherwise, <c>false</c>.
        /// </value>
        public abstract bool CanGoBack { get; }

        /// <summary>
        /// Gets a value indicating whether it is possible to navigate forward.
        /// </summary>
        /// <value>
        /// <c>true</c> if it is possible to navigate backforward otherwise, <c>false</c>.
        /// </value>
        public abstract bool CanGoForward { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Resolves the navigation target.
        /// </summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <returns>The target to navigate to.</returns>
        protected abstract string ResolveNavigationTarget(Type viewModelType);

        /// <summary>
        /// Returns the number of total back entries (which is the navigation history).
        /// </summary>
        public abstract int GetBackStackCount();

        /// <summary>
        /// Removes the last back entry from the navigation history.
        /// </summary>
        public abstract void RemoveBackEntry();

        /// <summary>
        /// Removes all the back entries from the navigation history.
        /// </summary>
        public abstract void RemoveAllBackEntries();
        #endregion
    }
}
