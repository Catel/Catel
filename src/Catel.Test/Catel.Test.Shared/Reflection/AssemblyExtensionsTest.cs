// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyExtensionsTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Reflection
{
    using System.Reflection;
    using Catel.Reflection;

    using NUnit.Framework;

    /// <summary>
    /// Summary description for AssemblyInfoTest
    /// </summary>
    [TestFixture]
    public class AssemblyExtensionsTest
    {
        private static readonly Assembly Assembly = typeof(AssemblyExtensionsTest).GetAssemblyEx();

        #region Methods
        [TestCase]
        public void TitleAutomatic()
        {
            string expected = "Catel.Test";

            var result = Assembly.Title();

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void VersionAutomatic()
        {
            string expected = "4.1.0.0";

            var result = Assembly.Version();

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void VersionWithSeparatorAutomatic()
        {
            string expected = "4.1";

            var result = Assembly.Version(1);

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void VersionWithSeparatorAutomaticWhereSeparatorCountIsTooHigh()
        {
            string expected = "4.1.0.0";

            var result = Assembly.Version(8);

            Assert.AreEqual(expected, result);
        }

        //[TestCase]
        //public void InformationalVersionAutomatic()
        //{
        //    var expected = "4.1, manually built in Visual Studio";

        //    var result = Assembly.InformationalVersion();

        //    Assert.AreEqual(expected, result);
        //}

        [TestCase]
        public void DescriptionAutomatic()
        {
            string expected = "Catel test library";

            var result = Assembly.Description();

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void ProductAutomatic()
        {
            string expected = "Catel.Test";

            var result = Assembly.Product();

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void CopyrightAutomatic()
        {
            string expected = "Copyright © CatenaLogic 2010 - 2014";

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

#if NET
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