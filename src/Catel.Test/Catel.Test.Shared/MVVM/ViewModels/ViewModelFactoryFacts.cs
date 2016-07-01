// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelFactoryFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.ViewModels
{
    using System;
    using System.Reflection;
    using Catel.IoC;
    using Catel.MVVM;
    using NUnit.Framework;

    public class ViewModelFactoryFacts
    {
        public interface IDummyDependency
        {
            string Value { get; set; }
        }

        public class DummyDependency : IDummyDependency
        {
            public string Value { get; set; }
        }

        public class TestViewModel : ViewModelBase
        {
            public TestViewModel()
            {
                EmptyConstructorCalled = true;
            }

            public TestViewModel(int integer)
            {
                Integer = integer;
            }

            public TestViewModel(bool boolean)
            {
                Boolean = boolean;
            }

            public TestViewModel(string stringvalue)
            {
                throw new NotSupportedException(stringvalue);
            }

            public TestViewModel(int integer, IDummyDependency dependency)
            {
                Integer = integer;
                Dependency = dependency;
            }

            public TestViewModel(IDummyDependency dependency)
            {
                Dependency = dependency;
            }

            public bool Boolean { get; set; }

            public int Integer { get; set; }

            public bool EmptyConstructorCalled { get; set; }

            public IDummyDependency Dependency { get; set; }
        }

        public class TestViewModelWithOnlyDefaultConstructor : TestViewModel
        {
            public TestViewModelWithOnlyDefaultConstructor()
            {
            }
        }

        [TestFixture]
        public class TheCreateViewModelMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullViewModelType()
            {
                var viewModelFactory = new ViewModelFactory(TypeFactory.Default);
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewModelFactory.CreateViewModel(null, null, null));
            }

            [TestCase]
            public void ThrowsExceptionCausedByInjectionConstructor()
            {
                var viewModelFactory = new ViewModelFactory(TypeFactory.Default);
                ExceptionTester.CallMethodAndExpectException<TargetInvocationException>(() => viewModelFactory.CreateViewModel<TestViewModel>("test", null),
                    e => string.Equals(e.InnerException.Message, "test"));
            }

            [TestCase]
            public void InstantiatesViewModelUsingInjectionForDataContext()
            {
                var viewModelFactory = new ViewModelFactory(TypeFactory.Default);
                var viewModel = viewModelFactory.CreateViewModel<TestViewModel>(5, null);

                Assert.IsFalse(viewModel.EmptyConstructorCalled);
                Assert.AreEqual(5, viewModel.Integer);
            }

            [TestCase]
            public void InstantiatesViewModelUsingDefaultConstructorForNullDataContext()
            {
                var viewModelFactory = new ViewModelFactory(TypeFactory.Default);
                var viewModel = viewModelFactory.CreateViewModel<TestViewModel>(null, null);

                Assert.IsTrue(viewModel.EmptyConstructorCalled);
            }

            [TestCase]
            public void InstantiatesViewModelUsingDefaultConstructorForDataContext()
            {
                var viewModelFactory = new ViewModelFactory(TypeFactory.Default);
                var viewModel = viewModelFactory.CreateViewModel<TestViewModelWithOnlyDefaultConstructor>(5, null);

                Assert.IsTrue(viewModel.EmptyConstructorCalled);
            }

            [TestCase]
            public void ResolvesUsingPreferredTag()
            {
                var serviceLocator = new ServiceLocator();

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

                var typeFactory = serviceLocator.ResolveType<ITypeFactory>();

                var viewModelFactory = new ViewModelFactory(typeFactory);
                var viewModel = viewModelFactory.CreateViewModel<TestViewModel>(null, "tag");

                Assert.IsTrue(ReferenceEquals(tagDependency, viewModel.Dependency));
            }

            [TestCase]
            public void ResolvesUsingPreferredTagAndDataContext()
            {
                var serviceLocator = new ServiceLocator();

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

                var typeFactory = serviceLocator.ResolveType<ITypeFactory>();

                var viewModelFactory = new ViewModelFactory(typeFactory);
                var viewModel = viewModelFactory.CreateViewModel<TestViewModel>(5, "tag");

                Assert.IsTrue(ReferenceEquals(tagDependency, viewModel.Dependency));
            }
        }
    }
}