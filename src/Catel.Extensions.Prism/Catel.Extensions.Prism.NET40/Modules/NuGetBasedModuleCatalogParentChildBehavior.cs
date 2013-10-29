// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetBasedModuleCatalogParentChildBehavior.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules
{
    using Catel.Modules.Interfaces;

    /// <summary>
    /// 
    /// </summary>
    public class NuGetBasedModuleCatalogParentChildBehavior
    {
        #region Fields
        /// <summary>
        /// The module catalog instance.
        /// </summary>
        private readonly INuGetBasedModuleCatalog _moduleCatalog;

        /// <summary>
        /// Ignore dependencies
        /// </summary>
        private bool _ignoreDependencies;

        /// <summary>
        /// Allow prerelease versions
        /// </summary>
        private bool _allowPrereleaseVersions;

        /// <summary>
        /// The output directory
        /// </summary>
        private string _outputDirectory;

        /// <summary>
        /// The packaged module id filter expression.
        /// </summary>
        private string _packagedModuleIdFilterExpression;
        #endregion

        #region Constructors

        /// <summary>
        /// The NuGetBasedModuleCatalogParentChildBehavior constructor.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleCatalog"/> is <c>null</c>.</exception>
        public NuGetBasedModuleCatalogParentChildBehavior(INuGetBasedModuleCatalog moduleCatalog)
        {
            Argument.IsNotNull(() => moduleCatalog);
            _moduleCatalog = moduleCatalog;
            
            OutputDirectory = "packages";
            PackagedModuleIdFilterExpression = string.Empty;
            AllowPrereleaseVersions = false;
            IgnoreDependencies = true;
        }
        #endregion

        #region Properties
        /// <summary>
        ///  Gets or sets the output directory.
        /// </summary>
        public string OutputDirectory
        {
            get { return _moduleCatalog.Parent != null ? _moduleCatalog.Parent.OutputDirectory : _outputDirectory; }
            set { _outputDirectory = value; }
        }

        /// <summary>
        /// Indicates whether the module catalog ignore the dependencies or not.
        /// </summary>
        public bool IgnoreDependencies
        {
            get { return _moduleCatalog.Parent != null ? _moduleCatalog.Parent.IgnoreDependencies : _ignoreDependencies; }
            set { _ignoreDependencies = value; }
        }

        /// <summary>
        /// Indicates whether the module catalog can download prerelease versions.
        /// </summary>
        public bool AllowPrereleaseVersions
        {
            get { return _moduleCatalog.Parent != null ? _moduleCatalog.Parent.AllowPrereleaseVersions : _allowPrereleaseVersions; }
            set { _allowPrereleaseVersions = value; }
        }

        /// <summary>
        /// The package module id filter expression.
        /// </summary>
        public string PackagedModuleIdFilterExpression
        {
            get { return _moduleCatalog.Parent != null ? _moduleCatalog.Parent.PackagedModuleIdFilterExpression : _packagedModuleIdFilterExpression; }
            set { _packagedModuleIdFilterExpression = string.IsNullOrEmpty(value) ? string.Empty : value.Trim(); }
        }
        #endregion
    }
}