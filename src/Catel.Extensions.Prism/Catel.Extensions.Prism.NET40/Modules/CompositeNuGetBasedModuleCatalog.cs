// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeNuGetBasedModuleCatalog.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules
{
    using System.Collections.Generic;
    using System.Linq;

    using Catel.Logging;
    using Catel.Modules.Extensions;
    using Catel.Modules.Interfaces;

    using Microsoft.Practices.Prism.Modularity;

    using NuGet;

    /// <summary>
    /// The composite CompositeNuGetBasedModuleCatalog.
    /// </summary>
    public class CompositeNuGetBasedModuleCatalog : CompositeModuleCatalog<INuGetBasedModuleCatalog>, INuGetBasedModuleCatalog
    {
        #region Constants
        /// <summary>
        /// The Log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        /// <summary>
        /// The CompositeNuGetBasedModuleCatalog constructor. 
        /// </summary>
        public CompositeNuGetBasedModuleCatalog()
        {
            OutputDirectory = "packages";
            PackagedModuleIdFilterExpression = string.Empty;
            AllowPrereleaseVersions = false;
            IgnoreDependencies = true;
        }
        #endregion

        #region INuGetBasedModuleCatalog Members
        /// <summary>
        /// Gets the modules.
        /// </summary>
        public override IEnumerable<ModuleInfo> Modules
        {
            get
            {
                SynchronizeRequiredModuleCatalogsProperties();

                var moduleInfos = base.Modules.ToList();

                var rawModuleInfos = new List<ModuleInfo>();
                foreach (INuGetBasedModuleCatalog moduleCatalog in ModuleCatalogs)
                {
                    rawModuleInfos.AddRange(moduleCatalog.Modules);
                }

                moduleInfos.AddRange(rawModuleInfos.GroupBy(info => info.GetPackageName().Id).Select(grouping => grouping.OrderByDescending(info => info.GetPackageName().Version).FirstOrDefault()).Where(firstOrDefault => firstOrDefault != null));

                return moduleInfos;
            }
        }

        /// <summary>
        /// Tries to create and install package request from the <paramref name="moduleInfo"/>.
        /// </summary>
        /// <param name="moduleInfo">
        /// The module info.
        /// </param>
        /// <param name="installPackageRequest">
        /// The install package request.
        /// </param>
        /// <returns>
        /// <c>true</c> whether the install package request is created, otherwise <c>false</c>
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleInfo"/> is <c>null</c>.</exception>
        public bool TryCreateInstallPackageRequest(ModuleInfo moduleInfo, out InstallPackageRequest installPackageRequest)
        {
            Argument.IsNotNull(() => moduleInfo);

            installPackageRequest = null;
            var moduleCatalogs = ModuleCatalogs;

            int i = 0;
            while (installPackageRequest == null && i < moduleCatalogs.Count)
            {
                var moduleCatalog = moduleCatalogs[i++];
                moduleCatalog.TryCreateInstallPackageRequest(moduleInfo, out installPackageRequest);
            }

            return installPackageRequest != null;
        }

        /// <summary>
        /// Gets or sets the packaged module id filter expression.
        /// </summary>
        public string PackagedModuleIdFilterExpression
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates whether the module catalog can download prerelease versions.
        /// </summary>
        public bool AllowPrereleaseVersions
        {
            get { return ModuleCatalogs.Any(catalog => catalog.AllowPrereleaseVersions); }
            set
            {
                foreach (INuGetBasedModuleCatalog moduleCatalog in ModuleCatalogs)
                {
                    moduleCatalog.AllowPrereleaseVersions = value;
                }
            }
        }

        /// <summary>
        /// Indicates whether the module catalog ignore the dependencies or not.
        /// </summary>
        public bool IgnoreDependencies
        {
            get { return ModuleCatalogs.All(catalog => catalog.IgnoreDependencies); }
            set
            {
                foreach (INuGetBasedModuleCatalog moduleCatalog in ModuleCatalogs)
                {
                    moduleCatalog.IgnoreDependencies = value;
                }
            }
        }

        /// <summary>
        /// Gets the output directory full path.
        /// </summary>
        public string OutputDirectoryFullPath
        {
            get { return ModuleCatalogs.Select(catalog => catalog.OutputDirectoryFullPath).FirstOrDefault(); }
        }

        /// <summary>
        ///    Gets or sets the output directory.
        /// </summary>
        public string OutputDirectory
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets the package repository.
        /// </summary>
        public IPackageRepository GetPackageRepository()
        {
            var compositePackageRepository = new CompositePackageRepository();
            foreach (var moduleCatalog in ModuleCatalogs)
            {
                compositePackageRepository.Add(moduleCatalog.GetPackageRepository());
            }

            return compositePackageRepository;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Synchronize module catalogs
        /// </summary>
        private void SynchronizeRequiredModuleCatalogsProperties()
        {
            Log.Debug("Synchronizing required module catalogs properties");

            foreach (INuGetBasedModuleCatalog moduleCatalog in ModuleCatalogs)
            {
                moduleCatalog.OutputDirectory = OutputDirectory;
                moduleCatalog.PackagedModuleIdFilterExpression = PackagedModuleIdFilterExpression;
            }
        }
        #endregion
    }
}