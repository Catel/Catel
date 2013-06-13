// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewLocatorTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM
{
    using System;
    using Catel.MVVM;
    using SpecialTest;
    using Test.Views;
    using Views;
    using Test.ViewModels;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ViewLocatorFacts
    {
        [TestClass]
        public class TheRegisterMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullTypeToResolve()
            {
                var viewLocator = new ViewLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewLocator.Register(null, typeof(FollowingNoNamingConventionView)));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullResolvedType()
            {
                var viewLocator = new ViewLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewLocator.Register(typeof(NoNamingConventionViewModel), null));
            }

            [TestMethod]
            public void RegistersNonExistingViewType()
            {
                var viewLocator = new ViewLocator();

                Assert.IsNull(viewLocator.ResolveView(typeof(FollowingNoNamingConventionView)));

                viewLocator.Register(typeof(FollowingNoNamingConventionView), typeof(NoNamingConventionViewModel));

                var resolvedView = viewLocator.ResolveView(typeof (FollowingNoNamingConventionView));
                Assert.AreEqual(typeof(NoNamingConventionViewModel), resolvedView);
            }

            [TestMethod]
            public void OverwritesExistingViewType()
            {
                var viewLocator = new ViewLocator();
                viewLocator.Register(typeof(FollowingNoNamingConventionView), typeof(NoNamingConventionViewModel));
                viewLocator.Register(typeof(FollowingNoNamingConventionView), typeof(NoNamingConventionViewModel2));

                var resolvedView = viewLocator.ResolveView(typeof(FollowingNoNamingConventionView));
                Assert.AreEqual(typeof(NoNamingConventionViewModel2), resolvedView);
            }
        }

        [TestClass]
        public class TheResolveViewMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullViewType()
            {
                var viewLocator = new ViewLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewLocator.ResolveView(null));
            }

            [TestMethod]
            public void ReturnsViewForViewEndingWithViewModel()
            {
                var viewLocator = new ViewLocator();
                var resolvedType = viewLocator.ResolveView(typeof(PersonViewModel));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(PersonView), resolvedType);
            }

            [TestMethod]
            public void ReturnsViewForNamingConventionWithUp()
            {
                var viewLocator = new ViewLocator();
                viewLocator.NamingConventions.Clear();
                viewLocator.NamingConventions.Add("[UP].Views.[VM]View");

                var resolvedType = viewLocator.ResolveView(typeof(PersonViewModel));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(PersonView), resolvedType);
            }

            [TestMethod]
            public void ResolvesViewFromCache()
            {
                var viewLocator = new ViewLocator();
                var resolvedType = viewLocator.ResolveView(typeof(PersonViewModel));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(PersonView), resolvedType);

                // Clear the naming conventions (so it *must* come from the cache)
                viewLocator.NamingConventions.Clear();

                resolvedType = viewLocator.ResolveView(typeof(PersonViewModel));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(PersonView), resolvedType);
            }
        }

        [TestClass]
        public class TheClearCacheMethod
        {
            [TestMethod]
            public void ClearsTheCache()
            {
                var viewLocator = new ViewLocator();
                var resolvedType = viewLocator.ResolveView(typeof(PersonViewModel));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(PersonView), resolvedType);

                // Clear the naming conventions (so it *must* come from the cache)
                viewLocator.NamingConventions.Clear();

                resolvedType = viewLocator.ResolveView(typeof(PersonViewModel));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(PersonView), resolvedType);

                // Clear the cache, now it should break
                viewLocator.ClearCache();

                resolvedType = viewLocator.ResolveView(typeof(PersonViewModel));

                Assert.IsNull(resolvedType);
            }
        }
    }
}