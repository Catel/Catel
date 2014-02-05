// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelLocatorTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM
{
    using System;
    using Catel.MVVM;
    using SpecialTest;
    using Test.ViewModels;
    using Test.Views;
    using ViewModels;
    using Views;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    namespace Views
    {
        public class MyNameViewer { }
    }

    namespace ViewModels
    {
        public class MyNameViewerViewModel { }
    }

    public class ViewModelLocatorFacts
    {
        [TestClass]
        public class TheRegisterMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullTypeToResolve()
            {
                var viewModelLocator = new ViewModelLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewModelLocator.Register(null, typeof(NoNamingConventionViewModel)));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullResolvedType()
            {
                var viewModelLocator = new ViewModelLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewModelLocator.Register(typeof(FollowingNoNamingConventionView), null));
            }

            [TestMethod]
            public void RegistersNonExistingViewType()
            {
                var viewModelLocator = new ViewModelLocator();

                Assert.IsNull(viewModelLocator.ResolveViewModel(typeof(FollowingNoNamingConventionView)));

                viewModelLocator.Register(typeof(FollowingNoNamingConventionView), typeof(NoNamingConventionViewModel));

                var resolvedViewModel = viewModelLocator.ResolveViewModel(typeof (FollowingNoNamingConventionView));
                Assert.AreEqual(typeof(NoNamingConventionViewModel), resolvedViewModel);
            }

            [TestMethod]
            public void OverwritesExistingViewType()
            {
                var viewModelLocator = new ViewModelLocator();
                viewModelLocator.Register(typeof(FollowingNoNamingConventionView), typeof(NoNamingConventionViewModel));
                viewModelLocator.Register(typeof(FollowingNoNamingConventionView), typeof(NoNamingConventionViewModel2));

                var resolvedViewModel = viewModelLocator.ResolveViewModel(typeof(FollowingNoNamingConventionView));
                Assert.AreEqual(typeof(NoNamingConventionViewModel2), resolvedViewModel);
            }
        }

        [TestClass]
        public class TheResolveViewModelMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullViewType()
            {
                var viewModelLocator = new ViewModelLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewModelLocator.ResolveViewModel(null));
            }

            [TestMethod]
            public void ReturnsViewModelForViewEndingWithView()
            {
                var viewModelLocator = new ViewModelLocator();
                var resolvedType = viewModelLocator.ResolveViewModel(typeof(PersonView));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(PersonViewModel), resolvedType);
            }

            [TestMethod]
            public void ReturnsViewModelForViewEndingWithControl()
            {
                var viewModelLocator = new ViewModelLocator();
                var resolvedType = viewModelLocator.ResolveViewModel(typeof(Controls.PersonControl));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(PersonViewModel), resolvedType);
            }

            [TestMethod]
            public void ReturnsViewModelForViewEndingWithWindow()
            {
                var viewModelLocator = new ViewModelLocator();
                var resolvedType = viewModelLocator.ResolveViewModel(typeof(Windows.PersonWindow));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(PersonViewModel), resolvedType);
            }

            [TestMethod]
            public void ReturnsViewModelForViewEndingWithPage()
            {
                var viewModelLocator = new ViewModelLocator();
                var resolvedType = viewModelLocator.ResolveViewModel(typeof(Pages.PersonPage));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(PersonViewModel), resolvedType);
            }

            [TestMethod]
            public void ReturnsViewModelForNamingConventionWithUp()
            {
                var viewModelLocator = new ViewModelLocator();
                viewModelLocator.NamingConventions.Clear();
                viewModelLocator.NamingConventions.Add("[UP].ViewModels.[VW]ViewModel");

                var resolvedType = viewModelLocator.ResolveViewModel(typeof(Pages.PersonPage));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(PersonViewModel), resolvedType);
            }

            [TestMethod]
            public void ResolvesViewModelFromCache()
            {
                var viewModelLocator = new ViewModelLocator();
                var resolvedType = viewModelLocator.ResolveViewModel(typeof(PersonView));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(PersonViewModel), resolvedType);

                // Clear the naming conventions (so it *must* come from the cache)
                viewModelLocator.NamingConventions.Clear();

                resolvedType = viewModelLocator.ResolveViewModel(typeof(PersonView));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(PersonViewModel), resolvedType);
            }

            [TestMethod]
            public void ResolvesMyNameViewerViewModelFromMyNameViewer()
            {
                var viewModelLocator = new ViewModelLocator();
                var resolvedType = viewModelLocator.ResolveViewModel(typeof(MyNameViewer));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(MyNameViewerViewModel), resolvedType);
            }
        }

        [TestClass]
        public class TheClearCacheMethod
        {
            [TestMethod]
            public void ClearsTheCache()
            {
                var viewModelLocator = new ViewModelLocator();
                var resolvedType = viewModelLocator.ResolveViewModel(typeof(PersonView));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(PersonViewModel), resolvedType);

                // Clear the naming conventions (so it *must* come from the cache)
                viewModelLocator.NamingConventions.Clear();

                resolvedType = viewModelLocator.ResolveViewModel(typeof(PersonView));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(PersonViewModel), resolvedType);

                // Clear the cache, now it should break
                viewModelLocator.ClearCache();

                resolvedType = viewModelLocator.ResolveViewModel(typeof(PersonView));

                Assert.IsNull(resolvedType);
            }
        }
    }
}