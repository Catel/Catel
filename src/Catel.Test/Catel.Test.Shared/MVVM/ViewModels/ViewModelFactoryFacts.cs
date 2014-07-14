// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelFactoryFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.ViewModels
{
    using System;
    using Catel.IoC;
    using Catel.MVVM;
    using NUnit.Framework;

    public class ViewModelFactoryFacts
    {
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

            public bool Boolean { get; set; }

            public int Integer { get; set; }

            public bool EmptyConstructorCalled { get; set; }
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
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => viewModelFactory.CreateViewModel(null, null));
            }

            [TestCase]
            public void ThrowsExceptionCausedByInjectionConstructor()
            {
                var viewModelFactory = new ViewModelFactory(TypeFactory.Default);
                ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => viewModelFactory.CreateViewModel<TestViewModel>("test"),
                    e => string.Equals(e.Message, "test"));
            }

            [TestCase]
            public void InstantiatesViewModelUsingInjectionForDataContext()
            {
                var viewModelFactory = new ViewModelFactory(TypeFactory.Default);
                var viewModel = viewModelFactory.CreateViewModel<TestViewModel>(5);

                Assert.IsFalse(viewModel.EmptyConstructorCalled);
                Assert.AreEqual(5, viewModel.Integer);
            }

            [TestCase]
            public void InstantiatesViewModelUsingDefaultConstructorForNullDataContext()
            {
                var viewModelFactory = new ViewModelFactory(TypeFactory.Default);
                var viewModel = viewModelFactory.CreateViewModel<TestViewModel>(null);

                Assert.IsTrue(viewModel.EmptyConstructorCalled);
            }

            [TestCase]
            public void InstantiatesViewModelUsingDefaultConstructorForDataContext()
            {
                var viewModelFactory = new ViewModelFactory(TypeFactory.Default);
                var viewModel = viewModelFactory.CreateViewModel<TestViewModelWithOnlyDefaultConstructor>(5);

                Assert.IsTrue(viewModel.EmptyConstructorCalled);
            }
        }
    }
}