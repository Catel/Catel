// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeNuGetBasedModuleCatalog.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#if NET

namespace Catel.Modules
{
    using System.Collections.Generic;
    using System.Linq;

    using Catel.Logging;

#if PRISM6
    using Prism.Modularity;
#else
    using Microsoft.Practices.Prism.Modularity;
#endif

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

        /// <summary>
        /// The default parent child behavior.
        /// </summary>
        private readonly NuGetBasedModuleCatalogParentChildBehavior _behavior;
        #endregion

        #region Constructors
        /// <summary>
        /// The CompositeNuGetBasedModuleCatalog constructor. 
        /// </summary>
        public CompositeNuGetBasedModuleCatalog()
        {
            _behavior = new NuGetBasedModuleCatalogParentChildBehavior(this);
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
                var moduleInfos = base.Modules.ToList();

                var rawModuleInfos = new List<ModuleInfo>();
                foreach (var moduleCatalog in ModuleCatalogs)
                {
                    EnsureParentChildRelationship(moduleCatalog);
                    rawModuleInfos.AddRange(moduleCatalog.Modules);
                }

                moduleInfos.AddRange(rawModuleInfos.GroupBy(info => info.GetPackageName().Id).Select(grouping => grouping.OrderByDescending(info => info.GetPackageName().Version).FirstOrDefault()).Where(firstOrDefault => firstOrDefault != null));

                return moduleInfos;
            }
        }

        /// <summary>
        ///  Gets or sets the output directory.
        /// </summary>
        public string OutputDirectory
        {
            get { return _behavior.OutputDirectory; }
            set { _behavior.OutputDirectory = value; }
        }

        /// <summary>
        /// Indicates whether the module catalog ignore the dependencies or not.
        /// </summary>
        public bool IgnoreDependencies
        {
            get { return _behavior.IgnoreDependencies; }
            set { _behavior.IgnoreDependencies = value; }
        }

        /// <summary>
        /// The package module id filter expression.
        /// </summary>
        public string PackagedModuleIdFilterExpression
        {
            get { return _behavior.PackagedModuleIdFilterExpression; }
            set { _behavior.PackagedModuleIdFilterExpression = value; }
        }

        /// <summary>
        /// Indicates whether the module catalog can download prerelease versions.
        /// </summary>
        public bool AllowPrereleaseVersions
        {
            get { return _behavior.AllowPrereleaseVersions; }
            set { _behavior.AllowPrereleaseVersions = value; }
        }

        /// <summary>
        /// Gets or sets the parent nuget based catalog.
        /// </summary>
        /// <value>The parent.</value>
        public INuGetBasedModuleCatalog Parent { get; set; }

        /// <summary>
        /// Gets the output directory full path.
        /// </summary>
        public string OutputDirectoryFullPath
        {
            get { return ModuleCatalogs.Select(catalog => catalog.OutputDirectoryFullPath).FirstOrDefault(); }
        }

        /// <summary>
        /// Tries to create and install package request from the <paramref name="moduleInfo" />.
        /// </summary>
        /// <param name="moduleInfo">The module info.</param>
        /// <param name="installPackageRequest">The install package request.</param>
        /// <returns><c>true</c> whether the install package request is created, otherwise <c>false</c></returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleInfo" /> is <c>null</c>.</exception>
        public bool TryCreateInstallPackageRequest(ModuleInfo moduleInfo, out InstallPackageRequest installPackageRequest)
        {
            Argument.IsNotNull(() => moduleInfo);

            installPackageRequest = null;
            var moduleCatalogs = ModuleCatalogs;

            int i = 0;
            while (installPackageRequest == null && i < moduleCatalogs.Count)
            {
                var moduleCatalog = moduleCatalogs[i++];
                EnsureParentChildRelationship(moduleCatalog);
                moduleCatalog.TryCreateInstallPackageRequest(moduleInfo, out installPackageRequest);
            }

            return installPackageRequest != null;
        }

        /// <summary>
        /// Gets the package repository.
        /// </summary>
        /// <returns>IEnumerable&lt;IPackageRepository&gt;.</returns>
        public IEnumerable<IPackageRepository> GetPackageRepositories()
        {
            var packageRepositories = new List<IPackageRepository>();

            foreach (var moduleCatalog in LeafCatalogs)
            {
                var nugetBasedModuleCatalog = moduleCatalog as NuGetBasedModuleCatalog;
                if (nugetBasedModuleCatalog != null)
                {
                    packageRepositories.AddRange(nugetBasedModuleCatalog.GetPackageRepositories());
                }
            }

            return packageRepositories;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Ensure parent child relationship
        /// </summary>
        /// <param name="catalog">The catalog.</param>
        private void EnsureParentChildRelationship(INuGetBasedModuleCatalog catalog)
        {
            Log.Debug("Ensuring parent child relationship");

            catalog.Parent = this;
        }
        #endregion
    }
}

#endif