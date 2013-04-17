// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    /// <summary>
    /// Interface to define a service.
    /// </summary>
    public interface IService
    {
        #region Properties
        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        /// <value>The name of the service.</value>
        string Name { get; }
        #endregion
    }
}