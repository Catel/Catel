namespace Catel.Tests.MVVM.ViewModels
{
    using System;
    using System.Reflection;
    using Catel.MVVM;
    using NUnit.Framework;
    using TestClasses;

    public partial class ViewModelFactoryFacts
    {
#if !FINISHED_MIGRATION
        [TestFixture]
        public class TheCreateViewModelMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullViewModelType()
            {
                var viewModelFactory = new ViewModelFactory(TypeFactory.Default, ServiceLocator.Default);
                Assert.Throws<ArgumentNullException>(() => viewModelFactory.CreateViewModel(null, null, null));
            }

            [TestCase]
            public void ThrowsExceptionCausedByInjectionConstructor()
            {
                var viewModelFactory = new ViewModelFactory(TypeFactory.Default, ServiceLocator.Default);
                Assert.Throws<TargetInvocationException>(() => viewModelFactory.CreateViewModel<TestClasses.ViewModelFactoryTestViewModel>("test", null), "test");
            }

            [TestCase]
            public void InstantiatesViewModelUsingInjectionForDataContext()
            {
                var viewModelFactory = new ViewModelFactory(TypeFactory.Default, ServiceLocator.Default);
                var viewModel = viewModelFactory.CreateViewModel<TestClasses.ViewModelFactoryTestViewModel>(5, null);

                Assert.That(viewModel.EmptyConstructorCalled, Is.False);
                Assert.That(viewModel.Integer, Is.EqualTo(5));
            }

            [TestCase]
            public void InstantiatesViewModelUsingDefaultConstructorForNullDataContext()
            {
                var viewModelFactory = new ViewModelFactory(TypeFactory.Default, ServiceLocator.Default);
                var viewModel = viewModelFactory.CreateViewModel<TestClasses.ViewModelFactoryTestViewModel>(null, null);

                Assert.That(viewModel.EmptyConstructorCalled, Is.True);
            }

            [TestCase]
            public void InstantiatesViewModelUsingDefaultConstructorForDataContext()
            {
                var viewModelFactory = new ViewModelFactory(TypeFactory.Default, ServiceLocator.Default);
                var viewModel = viewModelFactory.CreateViewModel<TestClasses.ViewModelFactoryTestViewModelWithOnlyDefaultConstructor>(5, null);

                Assert.That(viewModel.EmptyConstructorCalled, Is.True);
            }

            [TestCase]
            public void ResolvesUsingPreferredTag()
            {
                using (var serviceLocator = new ServiceLocator())
                {
                    var noTagDependency = new DummyDependency
                    {
                        Value = "no tag"
                    };

                    var tagDependency = new DummyDependency
                    {
                        Value = "tag"
                    };

                    serviceLocator.RegisterInstance<IDummyDependency>(noTagDependency);
                    serviceLocator.RegisterInstance<IDummyDependency>(tagDependency, "tag");

                    var typeFactory = serviceLocator.ResolveRequiredType<ITypeFactory>();

                    var viewModelFactory = new ViewModelFactory(typeFactory, serviceLocator);
                    var viewModel = viewModelFactory.CreateViewModel<TestClasses.ViewModelFactoryTestViewModel>(null, "tag");

                    Assert.That(ReferenceEquals(tagDependency, viewModel.Dependency), Is.True);
                }
            }

            [TestCase]
            public void ResolvesUsingPreferredTagAndDataContext()
            {
                using (var serviceLocator = new ServiceLocator())
                {
                    var noTagDependency = new DummyDependency
                    {
                        Value = "no tag"
                    };

                    var tagDependency = new DummyDependency
                    {
                        Value = "tag"
                    };

                    serviceLocator.RegisterInstance<IDummyDependency>(noTagDependency);
                    serviceLocator.RegisterInstance<IDummyDependency>(tagDependency, "tag");

                    var typeFactory = serviceLocator.ResolveRequiredType<ITypeFactory>();

                    var viewModelFactory = new ViewModelFactory(typeFactory, serviceLocator);
                    var viewModel = viewModelFactory.CreateViewModel<TestClasses.ViewModelFactoryTestViewModel>(5, "tag");

                    Assert.That(ReferenceEquals(tagDependency, viewModel.Dependency), Is.True);
                }
            }
        }
#endif
    }
}
