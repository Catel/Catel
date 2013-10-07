// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleManager.cs" company="">
//   
// </copyright>
// <summary>
//   The module manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules.ModuleManager
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Practices.Prism.Logging;
    using Microsoft.Practices.Prism.Modularity;

    /// <summary>
    /// The module manager.
    /// </summary>
    public sealed class ModuleManager : Microsoft.Practices.Prism.Modularity.ModuleManager
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleManager"/> class.
        /// </summary>
        /// <param name="moduleInitializer">
        /// The module initializer.
        /// </param>
        /// <param name="moduleCatalog">
        /// The module catalog.
        /// </param>
        /// <param name="loggerFacade">
        /// The logger facade.
        /// </param>
        public ModuleManager(
            IModuleInitializer moduleInitializer, 
            IModuleCatalog moduleCatalog, 
            ILoggerFacade loggerFacade)
            : base(moduleInitializer, moduleCatalog, loggerFacade)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the module type loaders.
        /// </summary>
        public override IEnumerable<IModuleTypeLoader> ModuleTypeLoaders
        {
            get
            {
                List<IModuleTypeLoader> moduleTypeLoaders = base.ModuleTypeLoaders.ToList();
                if (moduleTypeLoaders.All(loader => loader.GetType() != typeof(NuGetModuleTypeLoader)))
                {
                    moduleTypeLoaders.Add(new NuGetModuleTypeLoader(this.ModuleCatalog));
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