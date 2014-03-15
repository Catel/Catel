// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositePackageRepository.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules
{
    using System.Collections.Generic;
    using System.Linq;

    using NuGet;

    /// <summary>
    /// The composite module package repository.
    /// </summary>
    public sealed class CompositePackageRepository : IPackageRepository
    {
        #region Fields

        /// <summary>
        /// Packages repositories. 
        /// </summary>
        private readonly List<IPackageRepository> packagesRepository = new List<IPackageRepository>();
        #endregion

        #region IPackageRepository Members
        /// <summary>
        /// Gets the packages.
        /// </summary>
        /// <returns></returns>
        public IQueryable<IPackage> GetPackages()
        {
            return packagesRepository.SelectMany(packageRepository => packageRepository.GetPackages()).ToList().AsQueryable();
        }

        /// <summary>
        /// Adds the package.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void AddPackage(IPackage package)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Removes the package.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void RemovePackage(IPackage package)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        public string Source { get; private set; }

        /// <summary>
        /// Gets or sets the package save mode.
        /// </summary>
        /// <value>The package save mode.</value>
        public PackageSaveModes PackageSaveMode { get; set; }

        /// <summary>
        /// Gets whether the repository supports prerelease packages.
        /// </summary>
        public bool SupportsPrereleasePackages
        {
            get
            {
                return packagesRepository.Any(repository => repository.SupportsPrereleasePackages);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageRepository"></param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="packageRepository"/> is <c>null</c>.</exception>
        public void Add(IPackageRepository packageRepository)
        {
            Argument.IsNotNull(() => packageRepository);

            packagesRepository.Add(packageRepository);
        }
        #endregion
    }
}