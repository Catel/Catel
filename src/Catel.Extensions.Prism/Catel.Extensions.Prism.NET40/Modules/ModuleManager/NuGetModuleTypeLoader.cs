// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetModuleTypeLoader.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the NuGetModuleTypeLoader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Modules.ModuleManager
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Versioning;
    using System.Text.RegularExpressions;

    using Catel.Logging;
    using Catel.Modules.Extensions;

    using Microsoft.Practices.Prism.Modularity;

    using NuGet;

    /// <summary>
    /// The NuGet module type loader.
    /// </summary>
    public sealed class NuGetModuleTypeLoader : IModuleTypeLoader
    {
        #region Constants

        /// <summary>
        /// The type name pattern.
        /// </summary>
        private const string TypeNamePattern = "[^,]+,([^,]+).*";

        /// <summary>
        /// The log.
        /// </summary>
        private readonly static ILog Log = LogManager.GetCurrentClassLogger();


        #endregion

        #region Fields

        /// <summary>
        /// The install package requests.
        /// </summary>
        private readonly Dictionary<ModuleInfo, InstallPackageRequest> installPackageRequests = new Dictionary<ModuleInfo, InstallPackageRequest>();

        /// <summary>
        /// The module catalogs.
        /// </summary>
        private readonly ReadOnlyCollection<NuGetBasedModuleCatalog> moduleCatalogs;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetModuleTypeLoader"/> class. 
        /// </summary>
        /// <param name="moduleCatalog">
        /// </param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleCatalog"/> is <c>null</c>.</exception>
        public NuGetModuleTypeLoader(IModuleCatalog moduleCatalog)
        {
            Argument.IsNotNull(() => moduleCatalog);

            if (moduleCatalog is NuGetBasedModuleCatalog)
            {
                this.moduleCatalogs = new List<NuGetBasedModuleCatalog> { moduleCatalog as NuGetBasedModuleCatalog }.AsReadOnly();
            }

            if (moduleCatalog is CompositeModuleCatalog)
            {
                var compositeModuleCatalog = moduleCatalog as CompositeModuleCatalog;
                this.moduleCatalogs = compositeModuleCatalog.QueryItems.ToList().OfType<NuGetBasedModuleCatalog>().ToList().AsReadOnly();
            }
        }

        #endregion

        #region Public Events

        /// <summary>
        /// </summary>
        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;

        private void OnLoadModuleCompleted(LoadModuleCompletedEventArgs e)
        {
            var handler = this.LoadModuleCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// </summary>
        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;

        private void OnModuleDownloadProgressChanged(ModuleDownloadProgressChangedEventArgs e)
        {
            var handler = this.ModuleDownloadProgressChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The can load module type.
        /// </summary>
        /// <param name="moduleInfo">
        /// The module Info.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="moduleInfo"/> is <c>null</c>.
        /// </exception>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanLoadModuleType(ModuleInfo moduleInfo)
        {
            Argument.IsNotNull(() => moduleInfo);

            bool canLoad = false;
            PackageName packageName = moduleInfo.GetPackageName();
            if (packageName != null)
            {
                int i = 0;
                while (!canLoad && i < this.moduleCatalogs.Count)
                {
                    NuGetBasedModuleCatalog moduleCatalog = this.moduleCatalogs[i++];
                    IPackageRepository repository = moduleCatalog.GetPackageRepository();

                    IPackage package;
                    if (repository.TryFindPackage(packageName.Id, packageName.Version, out package))
                    {
                        /*IEnumerable<FrameworkName> supportedFrameworks = package.GetSupportedFrameworks();
                        if (supportedFrameworks == null || supportedFrameworks.Any(name => name.Identifier.Equals(moduleCatalog.FrameworkNameIdentifier)))
                        {
                            Log.Warning("The package may have no support for framework '{0}'", moduleCatalog.FrameworkNameIdentifier);
                        }*/
                    
                        this.installPackageRequests.Add(moduleInfo, new InstallPackageRequest(repository, package, moduleCatalog));
                        
                        canLoad = true;
                    }
                }
            }

            return canLoad;
        }

        /// <summary>
        /// The load module type.
        /// </summary>
        /// <param name="moduleInfo">
        /// The module Info.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="moduleInfo"/> is <c>null</c>.
        /// </exception>
        public void LoadModuleType(ModuleInfo moduleInfo)
        {
            // ReSharper disable once ImplicitlyCapturedClosure
            Argument.IsNotNull(() => moduleInfo);

            if (this.installPackageRequests.ContainsKey(moduleInfo))
            {
                InstallPackageRequest installPackageRequest = this.installPackageRequests[moduleInfo];

                string url = "file://" + installPackageRequest.OutputDirectory.Replace("\\", "/");

                var packageManager = new PackageManager(installPackageRequest.PackageRepository, installPackageRequest.OutputDirectory);
                packageManager.InstallPackage(installPackageRequest.Package, installPackageRequest.IgnoreDependencies, installPackageRequest.AllowPrereleaseVersions);

                var typeNameMatch = Regex.Match(moduleInfo.ModuleType, TypeNamePattern);
                if (!typeNameMatch.Success)
                {
                    throw new InvalidOperationException("The module type must be specified using a qualified name pattern");
                }

                string assemblyFileName = typeNameMatch.Groups[1].Value.Trim() + ".dll";
                string directoryName = installPackageRequest.Package.GetFullName().Replace(' ', '.');

                var fileModuleTypeLoader = new FileModuleTypeLoader();
                var assemblyFileRef = string.Format("{0}/{1}/lib/{2}/{3}", url, directoryName, installPackageRequest.FrameworkNameIdentifier, assemblyFileName);
                var fileModuleInfo = new ModuleInfo(moduleInfo.ModuleName, moduleInfo.ModuleType)
                                         {
                                             Ref = assemblyFileRef, 
                                             InitializationMode = moduleInfo.InitializationMode, 
                                             DependsOn = moduleInfo.DependsOn
                                         };

                fileModuleTypeLoader.LoadModuleCompleted += (sender, args) =>
                    {
                        moduleInfo.State = args.ModuleInfo.State;
                        this.OnLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, args.Error));
                    };

                fileModuleTypeLoader.LoadModuleType(fileModuleInfo);
            }
        }

        #endregion

        #region Methods



        #endregion
    }
}