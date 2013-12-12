// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Modules.ModuleManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Logging;
    using Microsoft.Practices.Prism.Logging;
    using Microsoft.Practices.Prism.Modularity;

    /// <summary>
    /// The module manager.
    /// </summary>
    public sealed class ModuleManager : Microsoft.Practices.Prism.Modularity.ModuleManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleManager" /> class.
        /// </summary>
        /// <param name="moduleInitializer">The module initializer.</param>
        /// <param name="moduleCatalog">The module catalog.</param>
        /// <param name="loggerFacade">The logger facade.</param>
        public ModuleManager(IModuleInitializer moduleInitializer, IModuleCatalog moduleCatalog, ILoggerFacade loggerFacade)
            : base(moduleInitializer, moduleCatalog, loggerFacade)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the module type loaders.
        /// </summary>
        public override IEnumerable<IModuleTypeLoader> ModuleTypeLoaders
        {
            get
            {
                var moduleTypeLoaders = base.ModuleTypeLoaders.ToList();
                if (moduleTypeLoaders.All(loader => loader.GetType() != typeof (NuGetModuleTypeLoader)))
                {
                    try
                    {
                        var nuGetModuleTypeLoader = new NuGetModuleTypeLoader(ModuleCatalog);
                        moduleTypeLoaders.Add(nuGetModuleTypeLoader);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to create the NuGetModuleTypeLoader");
                    }
                }

                return moduleTypeLoaders;
            }
            set
            {
                base.ModuleTypeLoaders = value;
            }
        }
        #endregion
    }
}