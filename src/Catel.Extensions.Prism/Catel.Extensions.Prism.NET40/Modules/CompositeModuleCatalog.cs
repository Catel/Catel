// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeModuleCatalog.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Catel.Logging;

    using Microsoft.Practices.Prism.Modularity;

    /// <summary>
    /// Allows the combination of serveral module catalogs into a single module catalog.
    /// </summary>
    /// <remarks>
    /// This class can be used to aggregate serveral <see cref="IModuleCatalog"/> instances and deal with them as one. 
    /// Dependency between cross catalog modules is allowed. 
    /// </remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// public class Bootstrapper : BootstrapperBase<Shell, CompositeModuleCatalog>
    /// {
    /// 	protected override void ConfigureModuleCatalog()
    /// 	{
    ///			ModuleCatalog.Add(new DirectoryModuleCatalog { ModulePath = @".\" + ModuleBase.ModulesDirectory});
    ///			ModuleCatalog.Add(new ConfigurationModuleCatalog());
    /// 	}
    /// }
    /// ]]>
    ///  </code>
    /// </example>
    public class CompositeModuleCatalog<TModuleCatalog> : ModuleCatalog where TModuleCatalog : IModuleCatalog
    {
        #region Fields

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The module catalog list.
        /// </summary>
        private readonly List<TModuleCatalog> _moduleCatalogs = new List<TModuleCatalog>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the items in the <see cref="ModuleCatalog"/>.
        /// </summary>
        /// <value>The items in all module catalogs.</value>
        public override IEnumerable<IModuleCatalogItem> QueryItems
        {
            get
            {
                foreach (IModuleCatalogItem moduleCatalogItem in Items)
                {
                    yield return moduleCatalogItem;
                }

                foreach (IModuleCatalog moduleCatalog in _moduleCatalogs)
                {
                    IEnumerable<IModuleCatalogItem> moduleCatalogItems = null;

                    if (moduleCatalog is ModuleCatalog)
                    {
                        moduleCatalogItems = (moduleCatalog as ModuleCatalog).QueryItems;
                    }
                    else if (moduleCatalog is Microsoft.Practices.Prism.Modularity.ModuleCatalog)
                    {
                        moduleCatalogItems = (moduleCatalog as Microsoft.Practices.Prism.Modularity.ModuleCatalog).Items;
                    }

                    if (moduleCatalogItems != null)
                    {
                        foreach (IModuleCatalogItem moduleCatalogItem in moduleCatalogItems)
                        {
                            yield return moduleCatalogItem;    
                        }
                    }
                }
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes the catalog, which may load and validate the modules. 
        /// </summary>
        /// <exception cref="ModularityException">
        /// When validation of the ModuleCatalog fails, because this method calls Validate(). 
        /// </exception>
        /// <remarks>
        /// This method tries to initialize every single module catalog and finally initialize it self.<br/>
        /// - The initialization fails of the module catalog parts with <see cref="ModularityException"/>) will be ignored. The catalog will run at the end it's own initialization allowing "cross module catalog" module dependencies.    
        /// </remarks>
        public override void Initialize()
        {
            _synchronizationContext.Execute(() =>
                {
                    foreach (var moduleCatalog in _moduleCatalogs)
                    {
                        try
                        {
                            moduleCatalog.Initialize();
                        }
                        catch (ModularityException e)
                        {
                            Log.Warning(e);
                        }
                    }
                });

            base.Initialize();
        }

        /// <summary>
        /// Add a module catalog.
        /// </summary>
        /// <param name="moduleCatalog">
        /// The module catalog.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="moduleCatalog"/> is <c>null</c>.
        /// </exception>
        public void Add(TModuleCatalog moduleCatalog)
        {
            Argument.IsNotNull("moduleCatalog", moduleCatalog);
            
            Log.Debug("Adding a module catalog to a composition");

            _synchronizationContext.Execute(() => _moduleCatalogs.Add(moduleCatalog));

            EnsureCatalogValidated();
        }

        /// <summary>
        /// The module catalogs.
        /// </summary>
        protected ReadOnlyCollection<TModuleCatalog> ModuleCatalogs 
        { 
            get
            {
                return _moduleCatalogs.AsReadOnly();
            } 
        } 

        #endregion
    }

    /// <summary>
    /// Allows the combination of serveral module catalogs into a single module catalog.
    /// </summary>
    /// <remarks>
    /// This class can be used to aggregate serveral <see cref="IModuleCatalog"/> instances and deal with them as one. 
    /// Dependency between cross catalog modules is allowed. 
    /// </remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// public class Bootstrapper : BootstrapperBase<Shell, CompositeModuleCatalog>
    /// {
    /// 	protected override void ConfigureModuleCatalog()
    /// 	{
    ///			ModuleCatalog.Add(new DirectoryModuleCatalog { ModulePath = @".\" + ModuleBase.ModulesDirectory});
    ///			ModuleCatalog.Add(new ConfigurationModuleCatalog());
    /// 	}
    /// }
    /// ]]>
    ///  </code>
    /// </example>
    public sealed class CompositeModuleCatalog : CompositeModuleCatalog<ModuleCatalog>
    {
    }
}