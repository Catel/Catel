namespace Catel.Tests.Windows
{
    using System;
    using Catel.Windows;

    using NUnit.Framework;

    public class ResourceHelperFacts
    {
        [TestFixture]
        public class TheGetResourceUriMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullResourceUri()
            {
                Assert.Throws<ArgumentException>(() => ResourceHelper.GetResourceUri(null, null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyResourceUri()
            {
                Assert.Throws<ArgumentException>(() => ResourceHelper.GetResourceUri(string.Empty, null));
            }

            [TestCase]
            public void ReturnsPackUriForMethodWithOnlyResourceUri()
            {
                string packUri = ResourceHelper.GetResourceUri("App.xaml");

                Assert.That(packUri, Is.EqualTo("pack://application:,,,/App.xaml"));
            }

            [TestCase]
            public void ReturnsPackUriForCurrentApplicationWithoutStartingSlash()
            {
                string packUri = ResourceHelper.GetResourceUri("App.xaml", null);

                Assert.That(packUri, Is.EqualTo("pack://application:,,,/App.xaml"));
            }

            [TestCase]
            public void ReturnsPackUriForCurrentApplicationWithStartingSlash()
            {
                string packUri = ResourceHelper.GetResourceUri("/App.xaml", null);

                Assert.That(packUri, Is.EqualTo("pack://application:,,,/App.xaml"));
            }

            [TestCase]
            public void ReturnsPackUriForOtherAssemblyWithStartingSlash()
            {
                string packUri = ResourceHelper.GetResourceUri("App.xaml", "Catel.MVVM");

                Assert.That(packUri, Is.EqualTo("pack://application:,,,/Catel.MVVM;component/App.xaml"));
            }

            [TestCase]
            public void ReturnsPackUriForOtherAssemblyWithoutStartingSlash()
            {
                string packUri = ResourceHelper.GetResourceUri("/App.xaml", "Catel.MVVM");

                Assert.That(packUri, Is.EqualTo("pack://application:,,,/Catel.MVVM;component/App.xaml"));
            }
        }

        [TestFixture]
        public class TheXamlPageExistsMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullString()
            {
                Assert.Throws<ArgumentException>(() => ResourceHelper.XamlPageExists((string)null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyString()
            {
                Assert.Throws<ArgumentException>(() => ResourceHelper.XamlPageExists(string.Empty));
            }

            [TestCase]
            public void ThrowsUriFormatExceptionForInvalidUriString()
            {
                Assert.Throws<UriFormatException>(() => ResourceHelper.XamlPageExists("pac://,test[]df`"));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullUri()
            {
                Assert.Throws<ArgumentNullException>(() => ResourceHelper.XamlPageExists((Uri)null));
            }

            [TestCase]
            public void ReturnsFalseForNonExistingResourceAsUriString()
            {
                Assert.That(ResourceHelper.XamlPageExists(ResourceHelper.GetResourceUri("NonExistingTestControl.xaml", "Catel.Tests")), Is.False);
            }

            [TestCase]
            public void ReturnsTrueForExistingResourceAsUriString()
            {
                Assert.That(ResourceHelper.XamlPageExists(ResourceHelper.GetResourceUri("TestControl.xaml", "Catel.Tests")), Is.True);
            }

            [TestCase]
            public void ReturnsFalseForNonExistingResourceAsUri()
            {
                ResourceHelper.EnsurePackUriIsAllowed();

                Assert.That(ResourceHelper.XamlPageExists(new Uri(ResourceHelper.GetResourceUri("NonExistingTestControl.xaml", "Catel.Tests"), UriKind.RelativeOrAbsolute)), Is.False);
            }

            [TestCase]
            public void ReturnsTrueForExistingResourceAsUri()
            {
                ResourceHelper.EnsurePackUriIsAllowed();

                Assert.That(ResourceHelper.XamlPageExists(new Uri(ResourceHelper.GetResourceUri("TestControl.xaml", "Catel.Tests"), UriKind.RelativeOrAbsolute)), Is.True);
            }
        }
    }
}
