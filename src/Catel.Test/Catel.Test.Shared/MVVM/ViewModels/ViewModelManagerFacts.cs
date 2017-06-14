// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelManagerTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.ViewModels
{
    using System;
    using System.Linq;
    using Catel.MVVM;
    using Test.ViewModels;
    using TestClasses;

    using NUnit.Framework;

    [TestFixture]
    public class ViewModelManagerFacts
    {
        [TestFixture]
        public class TheRegisterModelMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullViewModel()
            {
                var model = new Person();
                var vmManager = new ViewModelManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => vmManager.RegisterModel(null, model));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullModel()
            {
                var model = new Person();
                var vm = new TestViewModel(model);
                var vmManager = new ViewModelManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => vmManager.RegisterModel(vm, null));
            }

            [TestCase]
            public void RegistersModelForViewModel()
            {
                var model = new Person();
                var vm = new TestViewModel(model);
                var vmManager = new ViewModelManager();

                vmManager.RegisterViewModelInstance(vm);
                vmManager.RegisterModel(vm, model);

                var foundVm = vmManager.GetViewModelsOfModel(model).First();

                Assert.AreEqual(vm, foundVm);
            }
        }

        [TestFixture]
        public class TheUnregisterModelMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullViewModel()
            {
                var model = new Person();
                var vmManager = new ViewModelManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => vmManager.UnregisterModel(null, model));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullModel()
            {
                var model = new Person();
                var vm = new TestViewModel(model);
                var vmManager = new ViewModelManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => vmManager.UnregisterModel(vm, null));
            }

            [TestCase]
            public void UnregistersModelForViewModel()
            {
                var model = new Person();
                var vm = new TestViewModel(model);
                var vmManager = new ViewModelManager();

                vmManager.RegisterViewModelInstance(vm);
                vmManager.RegisterModel(vm, model);

                var foundVm = vmManager.GetViewModelsOfModel(model).First();

                Assert.AreEqual(vm, foundVm);

                vmManager.UnregisterModel(vm, model);

                foundVm = vmManager.GetViewModelsOfModel(model).FirstOrDefault();

                Assert.IsNull(foundVm);
            }
        }

        [TestFixture]
        public class TheUnregisterAllModelsMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullViewModel()
            {
                var vmManager = new ViewModelManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => vmManager.UnregisterAllModels(null));
            }

            [TestCase]
            public void UnregistersAllModelForViewModel()
            {
                var model = new Person();
                var vm = new TestViewModel(model);
                var vmManager = new ViewModelManager();

                vmManager.RegisterViewModelInstance(vm);
                vmManager.RegisterModel(vm, model);
                
                var foundVm = vmManager.GetViewModelsOfModel(model).First();

                Assert.AreEqual(vm, foundVm);

                vmManager.UnregisterAllModels(vm);

                foundVm = vmManager.GetViewModelsOfModel(model).FirstOrDefault();

                Assert.IsNull(foundVm);
            }
        }

        [TestFixture]
        public class TheGetViewModelsOfModelMethod
        {
            [TestCase]
            public void ReturnsNullForUnregisteredModel()
            {
                var model = new Person();
                var vmManager = new ViewModelManager();

                var foundVm = vmManager.GetViewModelsOfModel(model).FirstOrDefault();

                Assert.IsNull(foundVm);
            }

            [TestCase]
            public void ReturnsViewModelOfRegisteredModel()
            {
                var model = new Person();
                var vm = new TestViewModel(model);
                var vmManager = new ViewModelManager();

                vmManager.RegisterViewModelInstance(vm);
                vmManager.RegisterModel(vm, model);

                var foundVm = vmManager.GetViewModelsOfModel(model).First();

                Assert.AreEqual(vm, foundVm);
            }
        }

        [TestFixture]
        public class TheGetViewModelMethod
        {
            [TestCase]
            public void ReturnsNullForUnregisteredViewModel()
            {
                var vmManager = new ViewModelManager();

                var foundvm = vmManager.GetViewModel(42);

                Assert.IsNull(foundvm);
            }

            [TestCase]
            public void ReturnsViewModelForRegisteredViewModel()
            {
                var vm = new TestViewModel();
                var vmManager = new ViewModelManager();

                vmManager.RegisterViewModelInstance(vm);
                var foundvm = vmManager.GetViewModel(vm.UniqueIdentifier);

                Assert.AreEqual(vm, foundvm);
            }
        }

        [TestFixture]
        public class TheGetChildViewModelsMethod
        {
            [TestCase]
            public void ReturnsNullForUnregisteredChildViewModels()
            {
                var viewModelManager = new ViewModelManager();

                var foundViewModels = viewModelManager.GetChildViewModels(42);

                Assert.AreEqual(0, foundViewModels.Count());
            }

            [TestCase]
            public void ReturnsChildViewModelsUsingParentInstance()
            {
                var parentViewModel = new TestViewModel() as IRelationalViewModel;
                var childViewModel = new TestViewModel() as IRelationalViewModel;
                var viewModelManager = new ViewModelManager();

                parentViewModel.RegisterChildViewModel(childViewModel as IViewModel);
                childViewModel.SetParentViewModel(parentViewModel as IViewModel);

                viewModelManager.RegisterViewModelInstance(parentViewModel as IViewModel);
                viewModelManager.RegisterViewModelInstance(childViewModel as IViewModel);

                var foundViewModels = viewModelManager.GetChildViewModels(parentViewModel as IViewModel);

                Assert.IsNotNull(foundViewModels);
                Assert.IsTrue(foundViewModels.Contains(childViewModel));
            }
        }

        [TestFixture]
        public class TheGetFirstOrDefaultInstanceMethod
        {
            [TestCase]
            public void ReturnsNullForUnregisteredViewModel()
            {
                var vmManager = new ViewModelManager();

                var foundvm = vmManager.GetFirstOrDefaultInstance<TestViewModel>();

                Assert.IsNull(foundvm);
            }

            [TestCase]
            public void ReturnsViewModelForRegisteredViewModel()
            {
                var vm = new TestViewModel();
                var vmManager = new ViewModelManager();

                vmManager.RegisterViewModelInstance(vm);
                var foundvm = vmManager.GetFirstOrDefaultInstance<TestViewModel>();

                Assert.AreEqual(vm, foundvm);
            }

            [TestCase]
            public void ReturnsViewModelForMultiRegisteredViewModel()
            {
                var firstvm = new TestViewModel(){FirstName = "Rajiv", LastName = "Mounguengue"};
                var secondvm = new TestViewModel();
                var vmManager = new ViewModelManager();

                vmManager.RegisterViewModelInstance(firstvm);
                vmManager.RegisterViewModelInstance(secondvm);
                var foundvm = vmManager.GetFirstOrDefaultInstance<TestViewModel>();

                Assert.AreEqual(firstvm, foundvm);
            }

            [TestCase]
            public void ShouldFailsDueToANonIViewModelType()
            {
                var viewModelManager = new ViewModelManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => viewModelManager.GetFirstOrDefaultInstance(typeof(Type)));
            }
        }

        [TestFixture]
        public class TheActiveViewModelsMethod
        {
            [TestCase]
            public void MustBeNotNullAfterConstructed()
            {
                var vmManager = new ViewModelManager();

                Assert.IsNotNull(vmManager.ActiveViewModels);
            }

            [TestCase]
            public void MustExistsForRegisteredViewModels()
            {
                var firstvm = new TestViewModel();
                var secondvm = new TestViewModel();
                var vmManager = new ViewModelManager();

                vmManager.RegisterViewModelInstance(firstvm);
                vmManager.RegisterViewModelInstance(secondvm);

                var vmList = vmManager.ActiveViewModels.ToList();

                Assert.IsTrue(vmList.Any(vm => TagHelper.AreTagsEqual(vm.UniqueIdentifier, firstvm.UniqueIdentifier)));
                Assert.IsTrue(vmList.Any(vm => TagHelper.AreTagsEqual(vm.UniqueIdentifier, secondvm.UniqueIdentifier)));
            }
        }

        [TestCase]
        public void RegisterViewModelInstance_Null()
        {
            var manager = new ViewModelManager();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => manager.RegisterViewModelInstance(null));
        }

        [TestCase]
        public void RegisterViewModelInstance_ViewModel()
        {
            var manager = new ViewModelManager();
            manager.RegisterViewModelInstance(new TestViewModel());
        }

        [TestCase]
        public void UnregisterViewModelInstance_Null()
        {
            var manager = new ViewModelManager();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => manager.UnregisterViewModelInstance(null));
        }

        [TestCase]
        public void UnregisterViewModelInstance_ExistingViewModel()
        {
            var manager = new ViewModelManager();
            var viewModel = new TestViewModel();

            manager.RegisterViewModelInstance(viewModel);
            manager.UnregisterViewModelInstance(viewModel);
        }

        [TestCase]
        public void UnregisterViewModelInstance_NotExistingViewModel()
        {
            var manager = new ViewModelManager();
            manager.UnregisterViewModelInstance(new TestViewModel());
        }
    }
}