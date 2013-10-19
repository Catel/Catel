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
    /// Module info manager.
    /// </summary>
    public class ModuleInfoManager : IModuleInfoManager
    {
        #region Fields
		/// <summary>
		///	The lock.
		/// </summary>
		private readonly object _lock = new object();
        
        /// <summary>
        ///	The dependency resolver.
        /// </summary>
        private readonly IDependencyResolver _dependencyResolver;
        #endregion

        /// <summary>
        /// Initializes the ModuleInfoManager.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public ModuleInfoManager(IServiceLocator serviceLocator = null)
            : this((serviceLocator != null) ? serviceLocator.ResolveType<IDependencyResolver>() : null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleInfoManager"/> class.
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        public ModuleInfoManager(IDependencyResolver dependencyResolver = null)
        {
            if (dependencyResolver == null)
            {
                dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            }

            _dependencyResolver = dependencyResolver;
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
                    var catalog = _dependencyResolver.Resolve<IModuleCatalog>();
                    return catalog.Modules;
                }
            }
        }
        #endregion
    }
}