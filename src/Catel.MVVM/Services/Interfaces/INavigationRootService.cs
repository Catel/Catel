// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INavigationRootService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    /// <summary>
    /// Service to retrieve the navigation root in the application.
    /// </summary>
    public interface INavigationRootService
    {
        /// <summary>
        /// Gets the navigation root.
        /// </summary>
        /// <returns>System.Object.</returns>
        object GetNavigationRoot();
    }
}
