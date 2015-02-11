﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationAdapterBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Navigation
{
    /// <summary>
    /// Abstract base class to allow partial abstract methods.
    /// </summary>
    public abstract class NavigationAdapterBase
    {
        #region Methods
        /// <summary>
        /// Determines whether the navigation can be handled by this adapter.
        /// </summary>
        /// <returns><c>true</c> if the navigation can be handled by this adapter; otherwise, <c>false</c>.</returns>
        protected virtual bool CanHandleNavigation()
        {
            return true;
        }

        /// <summary>
        /// Gets the navigation URI for the target page.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>System.String.</returns>
        protected abstract string GetNavigationUri(object target);
        #endregion
    }
}