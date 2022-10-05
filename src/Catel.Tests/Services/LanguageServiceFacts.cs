namespace Catel.Tests.Services
{
    using System;
    using Catel.Services;
    using NUnit.Framework;

    public class LanguageServiceFacts
    {
        [TestFixture]
        public class TheRegisterLanguageSourceMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullLanguageSource()
            {
                var languageService = new LanguageService();

                Assert.Throws<ArgumentNullException>(() => languageService.RegisterLanguageSource(null));
            }
        }

        [TestFixture]
        public class TheGetStringMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullResourceName()
            {
                var languageService = new LanguageService();

                Assert.Throws<ArgumentException>(() => languageService.GetString(null));
            }

            [TestCase]
            public void ReturnsNullForNonExistingResource()
            {
                var languageService = new LanguageService();

                Assert.AreEqual(null, languageService.GetString("NonExistingResourceName"));
            }

            //[TestCase]
            //public void ReturnsStringForCoreAssembly()
            //{
            //    var languageService = new LanguageService();

            //    Assert.AreEqual("{0} has the following warnings:", languageService.GetString("WarningsFound"));
            //}

            //[TestCase]
            //public void ReturnsStringForMvvmAssembly()
            //{
            //    var languageService = new LanguageService();

            //    Assert.AreEqual("Warning", languageService.GetString("WarningTitle"));
            //}
        }
    }
}
