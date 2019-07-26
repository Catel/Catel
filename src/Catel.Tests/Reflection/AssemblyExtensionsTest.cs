// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyExtensionsTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.Reflection
{
    using System;
    using System.Reflection;
    using Catel.Reflection;

    using NUnit.Framework;

    /// <summary>
    /// Summary description for AssemblyInfoTest
    /// </summary>
    [TestFixture]
    public class AssemblyExtensionsTest
    {
        // Note: don't use DateTime.Now.Year because we want a specific build to always compile, even in the next year
        private static readonly string CurrentYear = DateTime.Today.Year.ToString();
        private const string VersionPrefix = "5.11";

        private static readonly Assembly Assembly = typeof(AssemblyExtensionsTest).GetAssemblyEx();

        #region Methods
        [TestCase]
        public void TitleAutomatic()
        {
            var expected = "Catel.Tests";

            var result = Assembly.Title();

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void VersionAutomatic()
        {
            string expected = VersionPrefix + ".0.0";

            var result = Assembly.Version();

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void VersionWithSeparatorAutomatic()
        {
            string expected = VersionPrefix;

            var result = Assembly.Version(1);

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void VersionWithSeparatorAutomaticWhereSeparatorCountIsTooHigh()
        {
            string expected = VersionPrefix + ".0.0";

            var result = Assembly.Version(8);

            Assert.AreEqual(expected, result);
        }

        //[TestCase]
        //public void InformationalVersionAutomatic()
        //{
        //    var expected = VersionPrefix + ", manually built in Visual Studio";

        //    var result = Assembly.InformationalVersion();

        //    Assert.AreEqual(expected, result);
        //}

        [TestCase]
        public void DescriptionAutomatic()
        {
            string expected = "Catel tests library";

            var result = Assembly.Description();

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void ProductAutomatic()
        {
            string expected = "Catel.Tests";

            var result = Assembly.Product();

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void CopyrightAutomatic()
        {
            string expected = "Copyright © CatenaLogic 2010 - " + CurrentYear;

            var result = Assembly.Copyright();

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void CompanyAutomatic()
        {
            string expected = "CatenaLogic";

            var result = Assembly.Company();

            Assert.AreEqual(expected, result);
        }

#if NET || NETCORE
        [TestCase]
        public void TheGetBuildDateTimeMethod()
        {
            // Just check if the call works
            var dateTime = Assembly.GetBuildDateTime();
        }
#endif
        #endregion
    }
}
