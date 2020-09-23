// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceHelperTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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

#if NET || NETCORE
                Assert.AreEqual("pack://application:,,,/App.xaml", packUri);
#else
                Assert.AreEqual("/App.xaml", packUri);
#endif
            }

            [TestCase]
            public void ReturnsPackUriForCurrentApplicationWithoutStartingSlash()
            {
                string packUri = ResourceHelper.GetResourceUri("App.xaml", null);

#if NET || NETCORE
                Assert.AreEqual("pack://application:,,,/App.xaml", packUri);
#else
                Assert.AreEqual("/App.xaml", packUri);
#endif
            }

            [TestCase]
            public void ReturnsPackUriForCurrentApplicationWithStartingSlash()
            {
                string packUri = ResourceHelper.GetResourceUri("/App.xaml", null);

#if NET || NETCORE
                Assert.AreEqual("pack://application:,,,/App.xaml", packUri);
#else
                Assert.AreEqual("/App.xaml", packUri);
#endif
            }

            [TestCase]
            public void ReturnsPackUriForOtherAssemblyWithStartingSlash()
            {
                string packUri = ResourceHelper.GetResourceUri("App.xaml", "Catel.MVVM");

#if NET || NETCORE
                Assert.AreEqual("pack://application:,,,/Catel.MVVM;component/App.xaml", packUri);
#else
                Assert.AreEqual("/Catel.MVVM;component/App.xaml", packUri);
#endif
            }

            [TestCase]
            public void ReturnsPackUriForOtherAssemblyWithoutStartingSlash()
            {
                string packUri = ResourceHelper.GetResourceUri("/App.xaml", "Catel.MVVM");

#if NET || NETCORE
                Assert.AreEqual("pack://application:,,,/Catel.MVVM;component/App.xaml", packUri);
#else
                Assert.AreEqual("/Catel.MVVM;component/App.xaml", packUri);
#endif
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

#if !NETFX_CORE
            [TestCase]
            public void ThrowsUriFormatExceptionForInvalidUriString()
            {
                Assert.Throws<UriFormatException>(() => ResourceHelper.XamlPageExists("pac://,test[]df`"));
            }
#endif

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullUri()
            {
                Assert.Throws<ArgumentNullException>(() => ResourceHelper.XamlPageExists((Uri)null));
            }

            [TestCase]
            public void ReturnsFalseForNonExistingResourceAsUriString()
            {
                Assert.IsFalse(ResourceHelper.XamlPageExists(ResourceHelper.GetResourceUri("NonExistingTestControl.xaml", "Catel.Tests")));
            }

            [TestCase]
            public void ReturnsTrueForExistingResourceAsUriString()
            {
                Assert.IsTrue(ResourceHelper.XamlPageExists(ResourceHelper.GetResourceUri("TestControl.xaml", "Catel.Tests")));
            }

            [TestCase]
            public void ReturnsFalseForNonExistingResourceAsUri()
            {
                ResourceHelper.EnsurePackUriIsAllowed();

                Assert.IsFalse(ResourceHelper.XamlPageExists(new Uri(ResourceHelper.GetResourceUri("NonExistingTestControl.xaml", "Catel.Tests"), UriKind.RelativeOrAbsolute)));
            }

            [TestCase]
            public void ReturnsTrueForExistingResourceAsUri()
            {
                ResourceHelper.EnsurePackUriIsAllowed();

                Assert.IsTrue(ResourceHelper.XamlPageExists(new Uri(ResourceHelper.GetResourceUri("TestControl.xaml", "Catel.Tests"), UriKind.RelativeOrAbsolute)));
            }
        }
    }
}
