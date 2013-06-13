// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInfoManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules.ModuleManager
{
    using System.Collections.Generic;
    using IoC;
    using Microsoft.Practices.Prism.Modularity;

    /// <summary>
    /// 
    /// </summary>
    public class ModuleInfoManager : IModuleInfoManager
    {
        #region Fields
		/// <summary>
		///	The lock.
		/// </summary>
		private readonly object _lock = new object();
        
        /// <summary>
        ///	The service locator.
        /// </summary>
        private readonly IServiceLocator _serviceLocator;
        #endregion
        
        /// <summary>
        /// Initializes the ModuleInfoManager.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        public ModuleInfoManager(IServiceLocator serviceLocator = null)
        {
        	_serviceLocator = serviceLocator ?? ServiceLocator.Default;
        }

        #region IModuleInfoManager Members
        /// <summary>
        /// Gets the known modules.
        /// </summary>
        public IEnumerable<ModuleInfo> KnownModules
        {
            get
            {
            	// Why this lock is required. Is here a race condition?
                lock (_lock)
                {
                    var catalog = _serviceLocator.ResolveType<IModuleCatalog>();

                    return catalog.Modules;
                }
            }
        }
        #endregion
    }
}