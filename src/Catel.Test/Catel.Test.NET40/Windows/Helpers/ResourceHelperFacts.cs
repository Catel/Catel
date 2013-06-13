// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceHelperTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Windows
{
    using System;
    using Catel.Windows;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ResourceHelperFacts
    {
        [TestClass]
        public class TheGetResourceUriMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullResourceUri()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => ResourceHelper.GetResourceUri(null, null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyResourceUri()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => ResourceHelper.GetResourceUri(string.Empty, null));
            }

            [TestMethod]
            public void ReturnsPackUriForMethodWithOnlyResourceUri()
            {
                string packUri = ResourceHelper.GetResourceUri("App.xaml");

#if NET
                Assert.AreEqual("pack://application:,,,/App.xaml", packUri);
#else
                Assert.AreEqual("/App.xaml", packUri);
#endif
            }

            [TestMethod]
            public void ReturnsPackUriForCurrentApplicationWithoutStartingSlash()
            {
                string packUri = ResourceHelper.GetResourceUri("App.xaml", null);

#if NET
                Assert.AreEqual("pack://application:,,,/App.xaml", packUri);
#else
                Assert.AreEqual("/App.xaml", packUri);
#endif
            }

            [TestMethod]
            public void ReturnsPackUriForCurrentApplicationWithStartingSlash()
            {
                string packUri = ResourceHelper.GetResourceUri("/App.xaml", null);

#if NET
                Assert.AreEqual("pack://application:,,,/App.xaml", packUri);
#else
                Assert.AreEqual("/App.xaml", packUri);
#endif
            }

            [TestMethod]
            public void ReturnsPackUriForOtherAssemblyWithStartingSlash()
            {
                string packUri = ResourceHelper.GetResourceUri("App.xaml", "Catel.MVVM");

#if NET
                Assert.AreEqual("pack://application:,,,/Catel.MVVM;component/App.xaml", packUri);
#else
                Assert.AreEqual("/Catel.MVVM;component/App.xaml", packUri);
#endif
            }

            [TestMethod]
            public void ReturnsPackUriForOtherAssemblyWithoutStartingSlash()
            {
                string packUri = ResourceHelper.GetResourceUri("/App.xaml", "Catel.MVVM");

#if NET
                Assert.AreEqual("pack://application:,,,/Catel.MVVM;component/App.xaml", packUri);
#else
                Assert.AreEqual("/Catel.MVVM;component/App.xaml", packUri);
#endif
            }
        }

        [TestClass]
        public class TheXamlPageExistsMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullString()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => ResourceHelper.XamlPageExists((string)null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyString()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => ResourceHelper.XamlPageExists(string.Empty));
            }

#if !NETFX_CORE
            [TestMethod]
            public void ThrowsUriFormatExceptionForInvalidUriString()
            {
                ExceptionTester.CallMethodAndExpectException<UriFormatException>(() => ResourceHelper.XamlPageExists("pac://,test[]df`"));
            }
#endif

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullUri()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ResourceHelper.XamlPageExists((Uri)null));
            }

            [TestMethod]
            public void ReturnsFalseForNonExistingResourceAsUriString()
            {
                Assert.IsFalse(ResourceHelper.XamlPageExists(ResourceHelper.GetResourceUri("MyApp.xaml", "Catel.Test")));
            }

            [TestMethod]
            public void ReturnsTrueForExistingResourceAsUriString()
            {
                Assert.IsTrue(ResourceHelper.XamlPageExists(ResourceHelper.GetResourceUri("App.xaml", "Catel.Test")));
            }

            [TestMethod]
            public void ReturnsFalseForNonExistingResourceAsUri()
            {
                ResourceHelper.EnsurePackUriIsAllowed();

                Assert.IsFalse(ResourceHelper.XamlPageExists(new Uri(ResourceHelper.GetResourceUri("MyApp.xaml", "Catel.Test"), UriKind.RelativeOrAbsolute)));
            }

            [TestMethod]
            public void ReturnsTrueForExistingResourceAsUri()
            {
                ResourceHelper.EnsurePackUriIsAllowed();

                Assert.IsTrue(ResourceHelper.XamlPageExists(new Uri(ResourceHelper.GetResourceUri("App.xaml", "Catel.Test"), UriKind.RelativeOrAbsolute)));
            }
        }
    }
}