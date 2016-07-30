// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInfoExtensions_Facts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET45

namespace Catel.Test.Extensions.Prism.Modules.Extensions
{
    using System;
    using Catel.Modules;

    using Microsoft.Practices.Prism.Modularity;
    using NUnit.Framework;

    [TestFixture]
    public class ModuleInfoExtensions_Facts
    {
        #region Nested type: The_GetPackageName_Method
        [TestFixture]
        public class The_GetPackageName_Method
        {
            #region Constants
            private const string Version = "1.0.0-BETA";

            private const string PackageId = "Catel.Examples.WPF.Prism.Modules.NuGetBasedModuleA";
            #endregion

            #region Methods
            [TestCase]
            public void Throws_ArgumentNullException_If_Self_Reference_IsNull()
            {
                ModuleInfo moduleInfo = null;
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => moduleInfo.GetPackageName());
            }

            [TestCase]
            public void Returns_The_PackageName_If_A_Valid_Ref_Value_Is_Specified_On_ModuleInfo_Property()
            {
                var moduleInfo = new ModuleInfo { Ref = PackageId };
                var packageName = moduleInfo.GetPackageName();
                Assert.IsNotNull(packageName);
            }

            [TestCase]
            public void Returns_The_PackageName_With_The_Id_Specified_On_The_Ref_ModuleInfo_Property()
            {
                var moduleInfo = new ModuleInfo { Ref = PackageId };
                var packageName = moduleInfo.GetPackageName();
                Assert.AreEqual(packageName.Id, PackageId);
            }


            [TestCase]
            public void Returns_The_PackageName_With_The_Id_Specified_On_The_Ref_ModuleInfo_Property_2()
            {
                var moduleInfo = new ModuleInfo { Ref = string.Format("{0}, {1}", PackageId, Version) };
                var packageName = moduleInfo.GetPackageName();
                Assert.AreEqual(packageName.Id, PackageId);
            }

            [TestCase]
            public void Returns_Null_If_The_Version_Number_Is_Specified_Incorrectly()
            {
                var moduleInfo = new ModuleInfo { Ref = "&#$%2435.234.5, asdfhalksdfhas" };
                var packageName = moduleInfo.GetPackageName();
                Assert.IsNull(packageName);
            }

            [TestCase]
            public void Returns_Null_If_The_Ref_Value_Is_Empty()
            {
                var moduleInfo = new ModuleInfo { Ref = string.Empty };
                var packageName = moduleInfo.GetPackageName();
                Assert.IsNull(packageName);
            }

            [TestCase]
            public void Returns_The_PackageName_With_The_Version_Specified_On_The_Ref_ModuleInfo_Property()
            {
                var moduleInfo = new ModuleInfo { Ref = string.Format("{0}, {1}", PackageId, Version) };
                var packageName = moduleInfo.GetPackageName();
                Assert.AreEqual(packageName.Version.ToString(), Version);
            }

            [TestCase]
            public void Returns_The_PackageName_With_Version_As_Null_If_The_Version_Number_Is_Not_Specified_In_The_Ref_ModuleInfo_Property()
            {
                var moduleInfo = new ModuleInfo { Ref = PackageId };
                var packageName = moduleInfo.GetPackageName();
                Assert.IsNull(packageName.Version);
            }
            #endregion
        }
        #endregion
    }
}

#endif