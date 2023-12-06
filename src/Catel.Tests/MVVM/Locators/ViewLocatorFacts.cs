namespace Catel.Tests.MVVM
{
    using System;
    using Catel.MVVM;
    using Locators.Fixtures.ViewModels;
    using Locators.Fixtures.Views;
    using SpecialTest;
    using Tests.Views;
    using Tests.ViewModels;

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
                Assert.Throws<ArgumentNullException>(() => viewLocator.Register(null, typeof(FollowingNoNamingConventionView)));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullResolvedType()
            {
                var viewLocator = new ViewLocator();
                Assert.Throws<ArgumentNullException>(() => viewLocator.Register(typeof(NoNamingConventionViewModel), null));
            }

            [TestCase]
            public void RegistersNonExistingViewType()
            {
                var viewLocator = new ViewLocator();

                Assert.That(viewLocator.ResolveView(typeof(FollowingNoNamingConventionView)), Is.Null);

                viewLocator.Register(typeof(FollowingNoNamingConventionView), typeof(NoNamingConventionViewModel));

                var resolvedView = viewLocator.ResolveView(typeof(FollowingNoNamingConventionView));
                Assert.That(resolvedView, Is.EqualTo(typeof(NoNamingConventionViewModel)));
            }

            [TestCase]
            public void OverwritesExistingViewType()
            {
                var viewLocator = new ViewLocator();
                viewLocator.Register(typeof(FollowingNoNamingConventionView), typeof(NoNamingConventionViewModel));
                viewLocator.Register(typeof(FollowingNoNamingConventionView), typeof(NoNamingConventionViewModel2));

                var resolvedView = viewLocator.ResolveView(typeof(FollowingNoNamingConventionView));
                Assert.That(resolvedView, Is.EqualTo(typeof(NoNamingConventionViewModel2)));
            }
        }

        [TestFixture]
        public class TheIsCompatibleMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullTypeToResolve()
            {
                var viewLocator = new ViewLocator();
                Assert.Throws<ArgumentNullException>(() => viewLocator.IsCompatible(null, typeof(FollowingNoNamingConventionView)));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullResolvedType()
            {
                var viewLocator = new ViewLocator();
                Assert.Throws<ArgumentNullException>(() => viewLocator.IsCompatible(typeof(NoNamingConventionViewModel), null));
            }

            [TestCase(typeof(MyNameViewer), true)]
            [TestCase(typeof(MyNameViewer2), true)]
            [TestCase(typeof(NonCompatibleView), false)]
            public void ReturnsCompatibleValues(Type viewType, bool expectedValue)
            {
                var viewLocator = new ViewLocator();

                viewLocator.Register(typeof(MyNameViewerViewModel), typeof(MyNameViewer));
                viewLocator.Register(typeof(MyNameViewerViewModel), typeof(MyNameViewer2));

                var isCompatible = viewLocator.IsCompatible(typeof(MyNameViewerViewModel), viewType);
                Assert.That(isCompatible, Is.EqualTo(expectedValue));
            }
        }

        [TestFixture]
        public class TheResolveViewMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullViewType()
            {
                var viewLocator = new ViewLocator();
                Assert.Throws<ArgumentNullException>(() => viewLocator.ResolveView(null));
            }

            [TestCase(typeof(PersonViewModel), typeof(PersonView), null)]
            [TestCase(typeof(PersonViewModel), typeof(PersonView), "[UP].Views.[VM]View")]
            [TestCase(typeof(SameNamespacePersonViewModel), typeof(SameNamespacePersonView), null)]
            [TestCase(typeof(SameNamespacePersonViewModel), typeof(SameNamespacePersonView), "[CURRENT].[VM]View")]
            public void ReturnsViewForViewModel(Type viewModelType, Type viewType, string? convention)
            {
                var viewLocator = new ViewLocator();

                if (!string.IsNullOrEmpty(convention))
                {
                    viewLocator.NamingConventions.Clear();
                    viewLocator.NamingConventions.Add(convention);
                }

                var resolvedType = viewLocator.ResolveView(viewModelType);

                Assert.IsNotNull(resolvedType);
                Assert.That(resolvedType, Is.EqualTo(viewType));
            }

            [TestCase]
            public void ResolvesViewFromCache()
            {
                var viewLocator = new ViewLocator();
                var resolvedType = viewLocator.ResolveView(typeof(PersonViewModel));

                Assert.IsNotNull(resolvedType);
                Assert.That(resolvedType, Is.EqualTo(typeof(PersonView)));

                // Clear the naming conventions (so it *must* come from the cache)
                viewLocator.NamingConventions.Clear();

                resolvedType = viewLocator.ResolveView(typeof(PersonViewModel));

                Assert.IsNotNull(resolvedType);
                Assert.That(resolvedType, Is.EqualTo(typeof(PersonView)));
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
                Assert.That(resolvedType, Is.EqualTo(typeof(PersonView)));

                // Clear the naming conventions (so it *must* come from the cache)
                viewLocator.NamingConventions.Clear();

                resolvedType = viewLocator.ResolveView(typeof(PersonViewModel));

                Assert.IsNotNull(resolvedType);
                Assert.That(resolvedType, Is.EqualTo(typeof(PersonView)));

                // Clear the cache, now it should break
                viewLocator.ClearCache();

                resolvedType = viewLocator.ResolveView(typeof(PersonViewModel));

                Assert.That(resolvedType, Is.Null);
            }
        }
    }
}
