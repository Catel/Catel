// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules
{
    using System;
    using IoC;

#if PRISM6
    using Prism.Modularity;
    using Prism.Regions;
#else
    using Microsoft.Practices.Prism.Modularity;
    using Microsoft.Practices.Prism.Regions;
#endif


    /// <summary>
    /// Base class to allow faster development of prism modules.
    /// </summary>
    /// <typeparam name="TContainer">The type of the IoC container.</typeparam>
#if NET 
    [Module]
#endif
    public abstract class ModuleBase<TContainer> : IModule
        where TContainer : class
    {
        private bool _initialized;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleBase{T}"/> class.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <param name="moduleTracker">The module tracker.</param>
        /// <param name="container">The container.</param>
        /// <exception cref="ArgumentException">The <paramref name="moduleName"/> is <c>null</c> or whitespace.</exception>
        protected ModuleBase(string moduleName, IModuleTracker moduleTracker = null, TContainer container = null)
        {
            Argument.IsNotNullOrWhitespace("moduleName", moduleName);

            ModuleName = moduleName;
            ModuleTracker = moduleTracker;
            Container = container;

            if (ModuleTracker != null)
            {
                ModuleTracker.RecordModuleConstructed(moduleName);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the IoC container.
        /// </summary>
        protected TContainer Container { get; private set; }

        /// <summary>
        /// Gets the region manager.
        /// </summary>
        protected IRegionManager RegionManager
        {
            get { return GetService<IRegionManager>(); }
        }

        /// <summary>
        /// Gets the module tracker.
        /// </summary>
        protected IModuleTracker ModuleTracker { get; private set; }

        /// <summary>
        /// Gets the name of the module.
        /// </summary>
        protected string ModuleName { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        public void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            OnInitializing();

            OnRegisterViewsAndTypes();

            OnInitialized();

            if (ModuleTracker != null)
            {
                ModuleTracker.RecordModuleInitialized(ModuleName);
            }
        }

        /// <summary>
        /// Called when the module is initializing.
        /// </summary>
        protected virtual void OnInitializing()
        {
        }

        /// <summary>
        /// Registers the views and types.
        /// </summary>
        protected virtual void OnRegisterViewsAndTypes()
        {
        }

        /// <summary>
        /// Called when the module has been initialized.
        /// </summary>
        protected virtual void OnInitialized()
        {
        }

        /// <summary>
        /// Gets the service from the <see cref="ServiceLocator"/>.
        /// </summary>
        /// <typeparam name="T">The type of the service to retrieve.</typeparam>
        /// <returns>The service retrieved from the <see cref="ServiceLocator"/>.</returns>
        protected abstract T GetService<T>();
        #endregion
    }
}