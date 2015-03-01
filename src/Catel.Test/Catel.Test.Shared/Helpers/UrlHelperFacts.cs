// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationEventArgsExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test
{
    using System;
    using NUnit.Framework;

    public class UrlHelperFacts
    {
        [TestFixture]
        public class TheGetSafeUriStringMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullUri()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => UrlHelper.GetSafeUriString(null));
            }

            [TestCase]
            public void ReturnsSafeUriString()
            {
                var inputUri = new Uri("/Views/MainPage.xaml", UriKind.RelativeOrAbsolute);
                var uri = UrlHelper.GetSafeUriString(inputUri);

                Assert.AreEqual("/Views/MainPage.xaml", uri);
            }

            // Test case for https://catelproject.atlassian.net/browse/CTL-240
            [TestCase]
            public void ReturnsSafeUriStringForUriWithMultipleStartingSlashes()
            {
                var inputUri = new Uri("//Views/MainPage.xaml", UriKind.RelativeOrAbsolute);
                var uri = UrlHelper.GetSafeUriString(inputUri);

                Assert.AreEqual("/Views/MainPage.xaml", uri);
            }
        }
    }
}