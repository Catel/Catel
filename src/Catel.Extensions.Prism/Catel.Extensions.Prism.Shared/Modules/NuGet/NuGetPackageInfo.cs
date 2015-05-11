// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Modules
{
    using NuGet;

    /// <summary>
    /// NuGet package info.
    /// </summary>
    public class NuGetPackageInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPackageInfo"/> class.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <param name="packageRepository">The package repository.</param>
        public NuGetPackageInfo(IPackage package, IPackageRepository packageRepository)
        {
            Argument.IsNotNull(() => package);
            Argument.IsNotNull(() => packageRepository);

            Package = package;
            PackageRepository = packageRepository;
        }

        /// <summary>
        /// Gets the package.
        /// </summary>
        /// <value>The package.</value>
        public IPackage Package { get; private set; }

        /// <summary>
        /// Gets or sets the package repository.
        /// </summary>
        /// <value>The package repository.</value>
        public IPackageRepository PackageRepository { get; set; }
    }
}

#endif