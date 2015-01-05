// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewLocatorTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
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

    using NUnit.Framework;

    public class ViewLocatorFacts
    {
        [TestFixture]
        public class TheRegisterMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullTypeToResolve()
            {
                var viewLocator = new ViewLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewLocator.Register(null, typeof(FollowingNoNamingConventionView)));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullResolvedType()
            {
                var viewLocator = new ViewLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewLocator.Register(typeof(NoNamingConventionViewModel), null));
            }

            [TestCase]
            public void RegistersNonExistingViewType()
            {
                var viewLocator = new ViewLocator();

                Assert.IsNull(viewLocator.ResolveView(typeof(FollowingNoNamingConventionView)));

                viewLocator.Register(typeof(FollowingNoNamingConventionView), typeof(NoNamingConventionViewModel));

                var resolvedView = viewLocator.ResolveView(typeof (FollowingNoNamingConventionView));
                Assert.AreEqual(typeof(NoNamingConventionViewModel), resolvedView);
            }

            [TestCase]
            public void OverwritesExistingViewType()
            {
                var viewLocator = new ViewLocator();
                viewLocator.Register(typeof(FollowingNoNamingConventionView), typeof(NoNamingConventionViewModel));
                viewLocator.Register(typeof(FollowingNoNamingConventionView), typeof(NoNamingConventionViewModel2));

                var resolvedView = viewLocator.ResolveView(typeof(FollowingNoNamingConventionView));
                Assert.AreEqual(typeof(NoNamingConventionViewModel2), resolvedView);
            }
        }

        [TestFixture]
        public class TheResolveViewMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullViewType()
            {
                var viewLocator = new ViewLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewLocator.ResolveView(null));
            }

            [TestCase]
            public void ReturnsViewForViewEndingWithViewModel()
            {
                var viewLocator = new ViewLocator();
                var resolvedType = viewLocator.ResolveView(typeof(PersonViewModel));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(PersonView), resolvedType);
            }

            [TestCase]
            public void ReturnsViewForNamingConventionWithUp()
            {
                var viewLocator = new ViewLocator();
                viewLocator.NamingConventions.Clear();
                viewLocator.NamingConventions.Add("[UP].Views.[VM]View");

                var resolvedType = viewLocator.ResolveView(typeof(PersonViewModel));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(PersonView), resolvedType);
            }

            [TestCase]
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

        [TestFixture]
        public class TheClearCacheMethod
        {
            [TestCase]
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