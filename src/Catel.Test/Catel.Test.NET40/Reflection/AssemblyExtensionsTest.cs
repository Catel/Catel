// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyExtensionsTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Reflection
{
    using System.Reflection;

    using Catel.Reflection;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    /// <summary>
    /// Summary description for AssemblyInfoTest
    /// </summary>
    [TestClass]
    public class AssemblyExtensionsTest
    {
        #region Methods
        /// <summary>
        /// Checks the title of the assembly automatically.
        /// </summary>
        [TestMethod]
        public void TitleAutomatic()
        {
            string expected = "Catel.Test";

            var result = MethodBase.GetCurrentMethod().DeclaringType.Assembly.Title();

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Checks the version of the assembly automatically.
        /// </summary>
        [TestMethod]
        public void VersionAutomatic()
        {
            string expected = "4.0.0.0";

            var result = MethodBase.GetCurrentMethod().DeclaringType.Assembly.Version();

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Checks the version with a specified separator of the assembly automatically.
        /// </summary>
        [TestMethod]
        public void VersionWithSeparatorAutomatic()
        {
            string expected = "4.0";

            var result = MethodBase.GetCurrentMethod().DeclaringType.Assembly.Version(1);

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Checks the version with a specified separator of the assembly automatically.
        /// </summary>
        [TestMethod]
        public void VersionWithSeparatorAutomaticWhereSeparatorCountIsTooHigh()
        {
            string expected = "4.0.0.0";

            var result = MethodBase.GetCurrentMethod().DeclaringType.Assembly.Version(8);

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Checks the informational version of the assembly automatically.
        /// </summary>
        [TestMethod]
        public void InformationalVersionAutomatic()
        {
            var expected = "4.0, manually built in Visual Studio";

            var result = MethodBase.GetCurrentMethod().DeclaringType.Assembly.InformationalVersion();

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Checks the description of the assembly automatically.
        /// </summary>
        [TestMethod]
        public void DescriptionAutomatic()
        {
            string expected = "Catel test library";

            var result = MethodBase.GetCurrentMethod().DeclaringType.Assembly.Description();

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Checks the product of the assembly automatically.
        /// </summary>
        [TestMethod]
        public void ProductAutomatic()
        {
            string expected = "Catel.Test";

            var result = MethodBase.GetCurrentMethod().DeclaringType.Assembly.Product();

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Checks the copyright of the assembly automatically.
        /// </summary>
        [TestMethod]
        public void CopyrightAutomatic()
        {
            string expected = "Copyright © CatenaLogic 2010 - 2014";

            var result = MethodBase.GetCurrentMethod().DeclaringType.Assembly.Copyright();

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Checks the company of the assembly automatically.
        /// </summary>
        [TestMethod]
        public void CompanyAutomatic()
        {
            string expected = "CatenaLogic";

            var result = MethodBase.GetCurrentMethod().DeclaringType.Assembly.Company();

            Assert.AreEqual(expected, result);
        }
        #endregion
    }
}