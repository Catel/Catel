// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageServiceFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Services
{
    using System;
    using Catel.Services;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class LanguageServiceFacts
    {
        [TestClass]
        public class TheRegisterLanguageSourceMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullLanguageSource()
            {
                var languageService = new LanguageService();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => languageService.RegisterLanguageSource(null));
            }
        }

        #region Nested type: TheGetStringMethod
        [TestClass]
        public class TheGetStringMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullResourceName()
            {
                var languageService = new LanguageService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => languageService.GetString(null));
            }

            [TestMethod]
            public void ReturnsNullForNonExistingResource()
            {
                var languageService = new LanguageService();

                Assert.AreEqual(null, languageService.GetString("NonExistingResourceName"));
            }

            [TestMethod]
            public void ReturnsStringForCoreAssembly()
            {
                var languageService = new LanguageService();

                Assert.AreEqual("{0} has the following warnings:", languageService.GetString("WarningsFound"));
            }

            [TestMethod]
            public void ReturnsStringForMvvmAssembly()
            {
                var languageService = new LanguageService();

                Assert.AreEqual("Warning", languageService.GetString("WarningTitle"));
                Assert.AreEqual("Warning", languageService.GetString("WarningTitle"));
            }
        }
        #endregion
    }
}