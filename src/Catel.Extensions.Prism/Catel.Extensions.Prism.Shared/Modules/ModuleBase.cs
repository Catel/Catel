// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules
{
    using System;
    using IoC;

    /// <summary>
    /// Base class to allow faster development of prism modules which uses the <see cref="IServiceLocator"/>
    /// as IoC container.
    /// </summary>
    public abstract class ModuleBase : ModuleBase<IServiceLocator>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleBase{T}"/> class.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <param name="moduleTracker">The module tracker.</param>
        /// <param name="container">The container.</param>
        /// <exception cref="ArgumentException">The <paramref name="moduleName"/> is <c>null</c> or whitespace.</exception>
        protected ModuleBase(string moduleName, IModuleTracker moduleTracker = null, IServiceLocator container = null)
            : base(moduleName, moduleTracker, container ?? ServiceLocator.Default)
        {
        }

        /// <summary>
        /// Gets the service from the <see cref="ServiceLocator"/>.
        /// </summary>
        /// <typeparam name="T">The type of the service to retrieve.</typeparam>
        /// <returns>The service retrieved from the <see cref="ServiceLocator"/>.</returns>
        protected override T GetService<T>()
        {
            return Container.ResolveType<T>();
        }
    }
}