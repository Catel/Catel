// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UrlLocatorTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.MVVM
{
    using System;
    using Catel.MVVM;
    using SpecialTest;
    using Views;
    using Tests.ViewModels;

    using NUnit.Framework;

    public class UrlLocatorFacts
    {
        [TestFixture]
        public class TheRegisterMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullTypeToResolve()
            {
                var urlLocator = new UrlLocator();
                Assert.Throws<ArgumentNullException>(() => urlLocator.Register(null, "/Views/PersonView.xaml"));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullResolvedType()
            {
                var urlLocator = new UrlLocator();
                Assert.Throws<ArgumentException>(() => urlLocator.Register(typeof(NoNamingConventionViewModel), null));
                Assert.Throws<ArgumentException>(() => urlLocator.Register(typeof(NoNamingConventionViewModel), string.Empty));
            }

            [TestCase]
            public void RegistersNonExistingViewType()
            {
                var urlLocator = new UrlLocator();

                Assert.IsNull(urlLocator.ResolveUrl(typeof(FollowingNoNamingConventionView)));

                urlLocator.Register(typeof(NoNamingConventionViewModel), "/App.xaml");

                var resolvedUri = urlLocator.ResolveUrl(typeof(NoNamingConventionViewModel));
                Assert.AreEqual("/App.xaml", resolvedUri);
            }

            [TestCase]
            public void OverwritesExistingViewType()
            {
                var urlLocator = new UrlLocator();
                urlLocator.Register(typeof(NoNamingConventionViewModel2), "/Views/NoNaming1.xaml");
                urlLocator.Register(typeof(NoNamingConventionViewModel2), "/App.xaml");

                var resolvedUri = urlLocator.ResolveUrl(typeof(NoNamingConventionViewModel2), false);
                Assert.AreEqual("/App.xaml", resolvedUri);
            }
        }

        [TestFixture]
        public class TheResolveUrlMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullViewType()
            {
                var urlLocator = new UrlLocator();
                Assert.Throws<ArgumentNullException>(() => urlLocator.ResolveUrl(null));
            }

            [TestCase(typeof(PersonViewModel), "/Views/Person.xaml")]
            public void ReturnsViewForViewEndingWithViewModel(Type viewModelType, string expectedValue)
            {
                var urlLocator = new UrlLocator();
                var resolvedType = urlLocator.ResolveUrl(viewModelType, false);

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(expectedValue, resolvedType);
            }

            [TestCase]
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

        [TestFixture]
        public class TheClearCacheMethod
        {
            [TestCase]
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