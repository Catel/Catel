// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageServiceFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Services
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

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => languageService.RegisterLanguageSource(null));
            }
        }

        #region Nested type: TheGetStringMethod
        [TestFixture]
        public class TheGetStringMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullResourceName()
            {
                var languageService = new LanguageService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => languageService.GetString(null));
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
        #endregion
    }
}