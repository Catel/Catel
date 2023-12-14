namespace Catel.Tests
{
    using System;
    using NUnit.Framework;

    public class UriExtensionsFacts
    {
        [TestFixture]
        public class TheGetSafeUriStringMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullUri()
            {
                Assert.Throws<ArgumentNullException>(() => UriExtensions.GetSafeUriString(null));
            }

            [TestCase]
            public void ReturnsSafeUriString()
            {
                var inputUri = new Uri("/Views/MainPage.xaml", UriKind.RelativeOrAbsolute);
                var uri = UriExtensions.GetSafeUriString(inputUri);

                Assert.That(uri, Is.EqualTo("/Views/MainPage.xaml"));
            }

            // Test case for https://catelproject.atlassian.net/browse/CTL-240
            [TestCase]
            public void ReturnsSafeUriStringForUriWithMultipleStartingSlashes()
            {
                var inputUri = new Uri("//Views/MainPage.xaml", UriKind.RelativeOrAbsolute);
                var uri = UriExtensions.GetSafeUriString(inputUri);

                Assert.That(uri, Is.EqualTo("/Views/MainPage.xaml"));
            }
        }
    }
}
