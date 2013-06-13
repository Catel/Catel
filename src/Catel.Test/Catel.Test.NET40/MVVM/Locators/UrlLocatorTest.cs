// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UrlLocatorTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM
{
    using System;
    using Catel.MVVM;
    using SpecialTest;
    using Views;
    using Test.ViewModels;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class UrlLocatorFacts
    {
        [TestClass]
        public class TheRegisterMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullTypeToResolve()
            {
                var urlLocator = new UrlLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => urlLocator.Register(null, "/Views/PersonView.xaml"));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullResolvedType()
            {
                var urlLocator = new UrlLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => urlLocator.Register(typeof(NoNamingConventionViewModel), null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => urlLocator.Register(typeof(NoNamingConventionViewModel), string.Empty));
            }

            [TestMethod]
            public void RegistersNonExistingViewType()
            {
                var urlLocator = new UrlLocator();

                Assert.IsNull(urlLocator.ResolveUrl(typeof(FollowingNoNamingConventionView)));

                urlLocator.Register(typeof(NoNamingConventionViewModel), "/App.xaml");

                var resolvedUri = urlLocator.ResolveUrl(typeof(NoNamingConventionViewModel));
                Assert.AreEqual("/App.xaml", resolvedUri);
            }

            [TestMethod]
            public void OverwritesExistingViewType()
            {
                var urlLocator = new UrlLocator();
                urlLocator.Register(typeof(NoNamingConventionViewModel2), "/Views/NoNaming1.xaml");
                urlLocator.Register(typeof(NoNamingConventionViewModel2), "/App.xaml");

                var resolvedUri = urlLocator.ResolveUrl(typeof(NoNamingConventionViewModel2), false);
                Assert.AreEqual("/App.xaml", resolvedUri);
            }
        }

        [TestClass]
        public class TheResolveUrlMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullViewType()
            {
                var urlLocator = new UrlLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => urlLocator.ResolveUrl(null));
            }

            [TestMethod]
            public void ReturnsViewForViewEndingWithViewModel()
            {
                var urlLocator = new UrlLocator();
                var resolvedType = urlLocator.ResolveUrl(typeof(PersonViewModel), false);

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual("/Views/Person.xaml", resolvedType);
            }

            [TestMethod]
            public void ResolvesViewFromCache()
            {
                var urlLocator = new UrlLocator();
                var resolvedType = urlLocator.ResolveUrl(typeof(PersonViewModel), false);

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual("/Views/Person.xaml", resolvedType);

                // Clear the naming conventions (so it *must* come from the cache)
                urlLocator.NamingConventions.Clear();

                resolvedType = urlLocator.ResolveUrl(typeof(PersonViewModel), false);

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual("/Views/Person.xaml", resolvedType);
            }
        }

        [TestClass]
        public class TheClearCacheMethod
        {
            [TestMethod]
            public void ClearsTheCache()
            {
                var urlLocator = new UrlLocator();
                var resolvedUrl = urlLocator.ResolveUrl(typeof(PersonViewModel), false);

                Assert.IsNotNull(resolvedUrl);
                Assert.AreEqual("/Views/Person.xaml", resolvedUrl);

                // Clear the naming conventions (so it *must* come from the cache)
                urlLocator.NamingConventions.Clear();

                resolvedUrl = urlLocator.ResolveUrl(typeof(PersonViewModel), false);

                Assert.IsNotNull(resolvedUrl);
                Assert.AreEqual("/Views/Person.xaml", resolvedUrl);

                // Clear the cache, now it should break
                urlLocator.ClearCache();

                resolvedUrl = urlLocator.ResolveUrl(typeof(PersonViewModel), false);

                Assert.IsNull(resolvedUrl);
            }
        }
    }
}