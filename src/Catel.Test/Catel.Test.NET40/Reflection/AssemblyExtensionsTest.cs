// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyExtensionsTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Reflection
{
    using System;
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

            string result = string.Empty;

            // Call function
            result = MethodBase.GetCurrentMethod().DeclaringType.Assembly.Title();

            // Validate result
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Checks the version of the assembly automatically.
        /// </summary>
        [TestMethod]
        public void VersionAutomatic()
        {
            string expected = "1.0.0.0";
            string result = string.Empty;

            // Call function
            result = MethodBase.GetCurrentMethod().DeclaringType.Assembly.Version();

            // Validate result
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Checks the version with a specified separator of the assembly automatically.
        /// </summary>
        [TestMethod]
        public void VersionWithSeparatorAutomatic()
        {
            string expected = "1.0";
            string result = string.Empty;

            // Call function
            result = MethodBase.GetCurrentMethod().DeclaringType.Assembly.Version(1);

            // Validate result
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Checks the version with a specified separator of the assembly automatically.
        /// </summary>
        [TestMethod]
        public void VersionWithSeparatorAutomaticWhereSeparatorCountIsTooHigh()
        {
            string expected = "1.0.0.0";
            string result = string.Empty;

            // Call function
            result = MethodBase.GetCurrentMethod().DeclaringType.Assembly.Version(8);

            // Validate result
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Checks the informational version of the assembly automatically.
        /// </summary>
        [TestMethod]
        public void InformationalVersionAutomatic()
        {
            Version expected = new Version("1.0");
            Version result = null;

            // Call function
            result = MethodBase.GetCurrentMethod().DeclaringType.Assembly.InformationalVersion();

            // Validate result
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Checks the description of the assembly automatically.
        /// </summary>
        [TestMethod]
        public void DescriptionAutomatic()
        {
            string expected = "Catel test library";

            string result = string.Empty;

            // Call function
            result = MethodBase.GetCurrentMethod().DeclaringType.Assembly.Description();

            // Validate result
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Checks the product of the assembly automatically.
        /// </summary>
        [TestMethod]
        public void ProductAutomatic()
        {
            string expected = "Catel.Test";

            string result = string.Empty;

            // Call function
            result = MethodBase.GetCurrentMethod().DeclaringType.Assembly.Product();

            // Validate result
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Checks the copyright of the assembly automatically.
        /// </summary>
        [TestMethod]
        public void CopyrightAutomatic()
        {
            // Declare variables
            string expected = "Copyright © CatenaLogic 2010 - 2012";
            string result = string.Empty;

            // Call function
            result = MethodBase.GetCurrentMethod().DeclaringType.Assembly.Copyright();

            // Validate result
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Checks the company of the assembly automatically.
        /// </summary>
        [TestMethod]
        public void CompanyAutomatic()
        {
            // Declare variables
            string expected = "CatenaLogic";
            string result = string.Empty;

            // Call function
            result = MethodBase.GetCurrentMethod().DeclaringType.Assembly.Company();

            // Validate result
            Assert.AreEqual(expected, result);
        }
        #endregion
    }
}