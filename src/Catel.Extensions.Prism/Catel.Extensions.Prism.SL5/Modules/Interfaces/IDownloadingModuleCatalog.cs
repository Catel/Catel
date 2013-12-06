// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDownloadingModuleCatalog.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules
{
    using System;
    using Microsoft.Practices.Prism.Modularity;

    /// <summary>
    /// Interfaces defining additional functionality of the <see cref="IModuleCatalog"/>.
    /// </summary>
    public interface IDownloadingModuleCatalog : IModuleCatalog
    {
        /// <summary>
        /// Loads a specific module.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <param name="completedCallback">The completed callback.</param>
        /// <exception cref="ArgumentException">The <paramref name="moduleName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="completedCallback"/> is <c>null</c>.</exception>
        void LoadModule(string moduleName, Action completedCallback);

        /// <summary>
        /// Occurs when the module catalog starts downloading a module.
        /// </summary>
        event EventHandler<ModuleEventArgs> ModuleDownloading;

        /// <summary>
        /// Occurs when the module catalog has finished downloading a module.
        /// </summary>
        event EventHandler<ModuleEventArgs> ModuleDownloaded;
    }
}