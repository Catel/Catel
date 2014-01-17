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
        /// 
        /// </summary>
        /// <param name="package"></param>
        public void AddPackage(IPackage package)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="package"></param>
        public void RemovePackage(IPackage package)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Source { get; private set; }

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