// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelLocatorTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
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

    using NUnit.Framework;

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
        [TestFixture]
        public class TheRegisterMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullTypeToResolve()
            {
                var viewModelLocator = new ViewModelLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewModelLocator.Register(null, typeof(NoNamingConventionViewModel)));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullResolvedType()
            {
                var viewModelLocator = new ViewModelLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewModelLocator.Register(typeof(FollowingNoNamingConventionView), null));
            }

            [TestCase]
            public void RegistersNonExistingViewType()
            {
                var viewModelLocator = new ViewModelLocator();

                Assert.IsNull(viewModelLocator.ResolveViewModel(typeof(FollowingNoNamingConventionView)));

                viewModelLocator.Register(typeof(FollowingNoNamingConventionView), typeof(NoNamingConventionViewModel));

                var resolvedViewModel = viewModelLocator.ResolveViewModel(typeof (FollowingNoNamingConventionView));
                Assert.AreEqual(typeof(NoNamingConventionViewModel), resolvedViewModel);
            }

            [TestCase]
            public void OverwritesExistingViewType()
            {
                var viewModelLocator = new ViewModelLocator();
                viewModelLocator.Register(typeof(FollowingNoNamingConventionView), typeof(NoNamingConventionViewModel));
                viewModelLocator.Register(typeof(FollowingNoNamingConventionView), typeof(NoNamingConventionViewModel2));

                var resolvedViewModel = viewModelLocator.ResolveViewModel(typeof(FollowingNoNamingConventionView));
                Assert.AreEqual(typeof(NoNamingConventionViewModel2), resolvedViewModel);
            }
        }

        [TestFixture]
        public class TheResolveViewModelMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullViewType()
            {
                var viewModelLocator = new ViewModelLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewModelLocator.ResolveViewModel(null));
            }

            [TestCase(typeof(PersonView), typeof(PersonViewModel))]
            [TestCase(typeof(Controls.PersonControl), typeof(PersonViewModel))]
            [TestCase(typeof(Pages.PersonPage), typeof(PersonViewModel))]
            [TestCase(typeof(Windows.PersonWindow), typeof(PersonViewModel))]
            [TestCase(typeof(SameNamespacePersonView), typeof(SameNamespacePersonViewModel))]
            public void ReturnsViewModelForView(Type viewType, Type viewModelType)
            {
                var viewModelLocator = new ViewModelLocator();
                var resolvedType = viewModelLocator.ResolveViewModel(viewType);

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(viewModelType, resolvedType);
            }

            [TestCase]
            public void ReturnsViewModelForNamingConventionWithUp()
            {
                var viewModelLocator = new ViewModelLocator();
                viewModelLocator.NamingConventions.Clear();
                viewModelLocator.NamingConventions.Add("[UP].ViewModels.[VW]ViewModel");

                var resolvedType = viewModelLocator.ResolveViewModel(typeof(Pages.PersonPage));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(PersonViewModel), resolvedType);
            }

            [TestCase]
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

            [TestCase]
            public void ResolvesMyNameViewerViewModelFromMyNameViewer()
            {
                var viewModelLocator = new ViewModelLocator();
                var resolvedType = viewModelLocator.ResolveViewModel(typeof(Catel.Test.MVVM.Views.MyNameViewer));

                Assert.IsNotNull(resolvedType);
                Assert.AreEqual(typeof(MyNameViewerViewModel), resolvedType);
            }
        }

        [TestFixture]
        public class TheClearCacheMethod
        {
            [TestCase]
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