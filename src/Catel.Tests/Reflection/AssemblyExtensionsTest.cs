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
        private static readonly string VersionPrefix = typeof(AssemblyExtensionsTest).Assembly.Version(2);

        private static readonly Assembly Assembly = typeof(AssemblyExtensionsTest).GetAssemblyEx();

        [TestCase]
        public void TitleAutomatic()
        {
            var expected = "Catel.Tests";

            var result = Assembly.Title();

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase]
        public void VersionAutomatic()
        {
            string expected = VersionPrefix + ".0";

            var result = Assembly.Version();

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase]
        public void VersionWithSeparatorAutomatic()
        {
            string expected = VersionPrefix.Substring(0, VersionPrefix.IndexOf(".", 2));

            var result = Assembly.Version(1);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase]
        public void VersionWithSeparatorAutomaticWhereSeparatorCountIsTooHigh()
        {
            string expected = VersionPrefix + ".0";

            var result = Assembly.Version(8);

            Assert.That(result, Is.EqualTo(expected));
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

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase]
        public void ProductAutomatic()
        {
            string expected = "Catel.Tests";

            var result = Assembly.Product();

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase]
        public void CopyrightAutomatic()
        {
            string expected = "Copyright © CatenaLogic 2010 - " + CurrentYear;

            var result = Assembly.Copyright();

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase]
        public void CompanyAutomatic()
        {
            string expected = "CatenaLogic";

            var result = Assembly.Company();

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase]
        public void TheGetBuildDateTimeMethod()
        {
            // Just check if the call works
            var dateTime = Assembly.GetBuildDateTime();
        }
    }
}
