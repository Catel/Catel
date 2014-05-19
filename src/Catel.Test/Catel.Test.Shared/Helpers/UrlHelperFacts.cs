// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationEventArgsExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test
{
    using System;
#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class UrlHelperFacts
    {
        [TestClass]
        public class TheGetSafeUriStringMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullUri()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => UrlHelper.GetSafeUriString(null));
            }

            [TestMethod]
            public void ReturnsSafeUriString()
            {
                var inputUri = new Uri("/Views/MainPage.xaml", UriKind.RelativeOrAbsolute);
                var uri = UrlHelper.GetSafeUriString(inputUri);

                Assert.AreEqual("/Views/MainPage.xaml", uri);
            }

            // Test case for https://catelproject.atlassian.net/browse/CTL-240
            [TestMethod]
            public void ReturnsSafeUriStringForUriWithMultipleStartingSlashes()
            {
                var inputUri = new Uri("//Views/MainPage.xaml", UriKind.RelativeOrAbsolute);
                var uri = UrlHelper.GetSafeUriString(inputUri);

                Assert.AreEqual("/Views/MainPage.xaml", uri);
            }
        }
    }
}