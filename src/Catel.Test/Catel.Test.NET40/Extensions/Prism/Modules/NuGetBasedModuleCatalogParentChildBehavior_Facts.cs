// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetBasedModuleCatalogParentChildBehavior_Facts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Extensions.Prism.Modules
{
    using System;

    using Catel.Modules;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class NuGetBasedModuleCatalogParentChildBehavior_Facts
    {
        #region Nested type: The_AllowPrereleaseVersions_Property
        [TestClass]
        public class The_AllowPrereleaseVersions_Property
        {
            #region Methods
            [TestMethod]
            public void Returns_The_Parent_Value()
            {
                var mock = new Mock<INuGetBasedModuleCatalog>();
                var parent = new Mock<INuGetBasedModuleCatalog>();
                parent.SetupGet(catalog => catalog.AllowPrereleaseVersions).Returns(true);
                mock.SetupGet(catalog => catalog.Parent).Returns(parent.Object);
                var nuGetBasedModuleCatalogParentChildBehavior = new NuGetBasedModuleCatalogParentChildBehavior(mock.Object) { AllowPrereleaseVersions = false };
                Assert.IsTrue(nuGetBasedModuleCatalogParentChildBehavior.AllowPrereleaseVersions);
            }

            [TestMethod]
            public void Returns_Its_Own_Value()
            {
                var mock = new Mock<INuGetBasedModuleCatalog>();
                mock.SetupGet(catalog => catalog.Parent).Returns((INuGetBasedModuleCatalog)null);
                var nuGetBasedModuleCatalogParentChildBehavior = new NuGetBasedModuleCatalogParentChildBehavior(mock.Object) { AllowPrereleaseVersions = false };
                Assert.IsFalse(nuGetBasedModuleCatalogParentChildBehavior.AllowPrereleaseVersions);
            }
            #endregion
        }
        #endregion

        #region Nested type: The_Constructor
        [TestClass]
        public class The_Constructor
        {
            #region Methods
            [TestMethod]
            public void Throws_ArgumentNullException_If_ModuleCatalog_Argument_Is_Null()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new NuGetBasedModuleCatalogParentChildBehavior(null));
            }

            [TestMethod]
            public void Succeeds_If_ModuleCatalog_Argument_Is_Not_Null()
            {
                var mock = new Mock<INuGetBasedModuleCatalog>();
                new NuGetBasedModuleCatalogParentChildBehavior(mock.Object);
            }
            #endregion
        }
        #endregion

        #region Nested type: The_IgnoreDependencies_Property
        [TestClass]
        public class The_IgnoreDependencies_Property
        {
            #region Methods
            [TestMethod]
            public void Returns_The_Parent_Value()
            {
                var mock = new Mock<INuGetBasedModuleCatalog>();
                var parent = new Mock<INuGetBasedModuleCatalog>();
                parent.SetupGet(catalog => catalog.IgnoreDependencies).Returns(false);
                mock.SetupGet(catalog => catalog.Parent).Returns(parent.Object);
                var nuGetBasedModuleCatalogParentChildBehavior = new NuGetBasedModuleCatalogParentChildBehavior(mock.Object) { IgnoreDependencies = true };
                Assert.IsFalse(nuGetBasedModuleCatalogParentChildBehavior.IgnoreDependencies);
            }

            [TestMethod]
            public void Returns_Its_Own_Value()
            {
                var mock = new Mock<INuGetBasedModuleCatalog>();
                mock.SetupGet(catalog => catalog.Parent).Returns((INuGetBasedModuleCatalog)null);
                var nuGetBasedModuleCatalogParentChildBehavior = new NuGetBasedModuleCatalogParentChildBehavior(mock.Object) { IgnoreDependencies = true };
                Assert.IsTrue(nuGetBasedModuleCatalogParentChildBehavior.IgnoreDependencies);
            }
            #endregion
        }
        #endregion

        #region Nested type: The_OutputDirectory_Property
        [TestClass]
        public class The_OutputDirectory_Property
        {
            #region Methods
            [TestMethod]
            public void Returns_The_Parent_Value()
            {
                var mock = new Mock<INuGetBasedModuleCatalog>();
                var parent = new Mock<INuGetBasedModuleCatalog>();
                parent.SetupGet(catalog => catalog.OutputDirectory).Returns("packages");
                mock.SetupGet(catalog => catalog.Parent).Returns(parent.Object);
                var nuGetBasedModuleCatalogParentChildBehavior = new NuGetBasedModuleCatalogParentChildBehavior(mock.Object) { OutputDirectory = @"c:\tmp" };
                Assert.AreEqual("packages", nuGetBasedModuleCatalogParentChildBehavior.OutputDirectory);
            }

            [TestMethod]
            public void Returns_Its_Own_Value()
            {
                var mock = new Mock<INuGetBasedModuleCatalog>();
                mock.SetupGet(catalog => catalog.Parent).Returns((INuGetBasedModuleCatalog)null);
                var nuGetBasedModuleCatalogParentChildBehavior = new NuGetBasedModuleCatalogParentChildBehavior(mock.Object) { OutputDirectory = @"c:\tmp" };
                Assert.AreEqual(@"c:\tmp", nuGetBasedModuleCatalogParentChildBehavior.OutputDirectory);
            }
            #endregion
        }
        #endregion

        #region Nested type: The_PackagedModuleIdFilterExpression_Property
        [TestClass]
        public class The_PackagedModuleIdFilterExpression_Property
        {
            #region Methods
            [TestMethod]
            public void Returns_The_Parent_Value()
            {
                var mock = new Mock<INuGetBasedModuleCatalog>();
                var parent = new Mock<INuGetBasedModuleCatalog>();
                parent.SetupGet(catalog => catalog.PackagedModuleIdFilterExpression).Returns("Orchestra.Modules");
                mock.SetupGet(catalog => catalog.Parent).Returns(parent.Object);
                var nuGetBasedModuleCatalogParentChildBehavior = new NuGetBasedModuleCatalogParentChildBehavior(mock.Object) { PackagedModuleIdFilterExpression = "My.Modules" };
                Assert.AreEqual("Orchestra.Modules", nuGetBasedModuleCatalogParentChildBehavior.PackagedModuleIdFilterExpression);
            }

            [TestMethod]
            public void Returns_Its_Own_Value()
            {
                var mock = new Mock<INuGetBasedModuleCatalog>();
                mock.SetupGet(catalog => catalog.Parent).Returns((INuGetBasedModuleCatalog)null);
                var nuGetBasedModuleCatalogParentChildBehavior = new NuGetBasedModuleCatalogParentChildBehavior(mock.Object) { PackagedModuleIdFilterExpression = "My.Modules" };
                Assert.AreEqual("My.Modules", nuGetBasedModuleCatalogParentChildBehavior.PackagedModuleIdFilterExpression);
            }
            #endregion
        }
        #endregion
    }
}