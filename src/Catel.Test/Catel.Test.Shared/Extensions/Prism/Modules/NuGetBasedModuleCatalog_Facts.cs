// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetBasedModuleCatalogExtensions_Facts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Extensions.Prism.Modules
{
    using Catel.Modules;

    using NUnit.Framework;

    [TestFixture]
    public class NuGetBasedModuleCatalog_Facts
    {
        #region Nested type: The_GetPackageRepository_Method
        [TestFixture]
        public class The_GetPackageRepository_Method
        {
            [TestCase]
            public void Returns_Null_If_The_PackageSource_Is_Empty()
            {
                var nuGetBasedModuleCatalog = new NuGetBasedModuleCatalog { PackageSource = string.Empty };
                var packageRepository = nuGetBasedModuleCatalog.GetPackageRepository();
                Assert.IsNull(packageRepository);
            }

            [TestCase]
            public void Returns_Null_If_The_PackageSource_Has_Incorrect_Format()
            {
                var nuGetBasedModuleCatalog = new NuGetBasedModuleCatalog { PackageSource = "2344:2345982345:" };
                var packageRepository = nuGetBasedModuleCatalog.GetPackageRepository();
                Assert.IsNull(packageRepository);
            }
        }
        #endregion
    }
}

#endif