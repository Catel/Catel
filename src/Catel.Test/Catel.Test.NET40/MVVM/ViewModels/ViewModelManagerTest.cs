// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelManagerTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.ViewModels
{
    using System;
    using System.Linq;
    using Catel.MVVM;
    using Test.ViewModels;
    using TestClasses;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class ViewModelManagerTest
    {
        [TestClass]
        public class TheRegisterModelMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullViewModel()
            {
                var model = new Person();
                var vmManager = new ViewModelManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => vmManager.RegisterModel(null, model));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullModel()
            {
                var model = new Person();
                var vm = new TestViewModel(model);
                var vmManager = new ViewModelManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => vmManager.RegisterModel(vm, null));
            }

            [TestMethod]
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

        [TestClass]
        public class TheUnregisterModelMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullViewModel()
            {
                var model = new Person();
                var vmManager = new ViewModelManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => vmManager.UnregisterModel(null, model));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullModel()
            {
                var model = new Person();
                var vm = new TestViewModel(model);
                var vmManager = new ViewModelManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => vmManager.UnregisterModel(vm, null));
            }

            [TestMethod]
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

        [TestClass]
        public class TheUnregisterAllModelsMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullViewModel()
            {
                var vmManager = new ViewModelManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => vmManager.UnregisterAllModels(null));
            }

            [TestMethod]
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

        [TestClass]
        public class TheGetViewModelsOfModelMethod
        {
            [TestMethod]
            public void ReturnsNullForUnregisteredModel()
            {
                var model = new Person();
                var vmManager = new ViewModelManager();

                var foundVm = vmManager.GetViewModelsOfModel(model).FirstOrDefault();

                Assert.IsNull(foundVm);
            }

            [TestMethod]
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

        [TestClass]
        public class TheGetViewModelMethod
        {
            [TestMethod]
            public void ReturnsNullForUnregisteredViewModel()
            {
                var vmManager = new ViewModelManager();

                var foundvm = vmManager.GetViewModel(42);

                Assert.IsNull(foundvm);
            }

            [TestMethod]
            public void ReturnsViewModelForRegisteredViewModel()
            {
                var vm = new TestViewModel();
                var vmManager = new ViewModelManager();

                vmManager.RegisterViewModelInstance(vm);
                var foundvm = vmManager.GetViewModel(vm.UniqueIdentifier);

                Assert.AreEqual(vm, foundvm);
            }
        }

        [TestClass]
        public class TheGetChildViewModelsMethod
        {
            [TestMethod]
            public void ReturnsNullForUnregisteredChildViewModels()
            {
                var viewModelManager = new ViewModelManager();

                var foundViewModels = viewModelManager.GetChildViewModels(42);

                Assert.AreEqual(0, foundViewModels.Count());
            }

            [TestMethod]
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

        [TestClass]
        public class TheGetFirstOrDefaultInstanceMethod
        {
            [TestMethod]
            public void ReturnsNullForUnregisteredViewModel()
            {
                var vmManager = new ViewModelManager();

                var foundvm = vmManager.GetFirstOrDefaultInstance<TestViewModel>();

                Assert.IsNull(foundvm);
            }

            [TestMethod]
            public void ReturnsViewModelForRegisteredViewModel()
            {
                var vm = new TestViewModel();
                var vmManager = new ViewModelManager();

                vmManager.RegisterViewModelInstance(vm);
                var foundvm = vmManager.GetFirstOrDefaultInstance<TestViewModel>();

                Assert.AreEqual(vm, foundvm);
            }

            [TestMethod]
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

            [TestMethod]
            public void ShouldFailsDueToANonIViewModelType()
            {
                var viewModelManager = new ViewModelManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => viewModelManager.GetFirstOrDefaultInstance(typeof(Type)));
            }
        }

        [TestClass]
        public class TheActiveViewModelsMethod
        {
            [TestMethod]
            public void MustBeNotNullAfterConstructed()
            {
                var vmManager = new ViewModelManager();

                Assert.IsNotNull(vmManager.ActiveViewModels);
            }

            [TestMethod]
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

        [TestMethod]
        public void RegisterViewModelInstance_Null()
        {
            var manager = new ViewModelManager();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => manager.RegisterViewModelInstance(null));
        }

        [TestMethod]
        public void RegisterViewModelInstance_ViewModel()
        {
            var manager = new ViewModelManager();
            manager.RegisterViewModelInstance(new InterestingViewModel());
        }

        [TestMethod]
        public void UnregisterViewModelInstance_Null()
        {
            var manager = new ViewModelManager();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => manager.UnregisterViewModelInstance(null));
        }

        [TestMethod]
        public void UnregisterViewModelInstance_ExistingViewModel()
        {
            var manager = new ViewModelManager();
            var viewModel = new InterestingViewModel();

            manager.RegisterViewModelInstance(viewModel);
            manager.UnregisterViewModelInstance(viewModel);
        }

        [TestMethod]
        public void UnregisterViewModelInstance_NotExistingViewModel()
        {
            var manager = new ViewModelManager();
            manager.UnregisterViewModelInstance(new InterestingViewModel());
        }

        [TestMethod]
        public void AddInterestedViewModelInstance_FirstArgumentNull()
        {
            var manager = new ViewModelManager();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => manager.AddInterestedViewModelInstance(null, null));
        }

        [TestMethod]
        public void AddInterestedViewModelInstance_SecondArgumentNull()
        {
            var manager = new ViewModelManager();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => manager.AddInterestedViewModelInstance(typeof(InterestingViewModel), null));
        }

        [TestMethod]
        public void AddInterestedViewModelInstance_ViewModelForExistingInterestedInViewModel()
        {
            var manager = new ViewModelManager();

            var interestingViewModel = new InterestingViewModel();
            var interestedViewModel = new InterestedViewModel();

            manager.RegisterViewModelInstance(interestingViewModel);
            manager.AddInterestedViewModelInstance(typeof(InterestingViewModel), interestedViewModel);
        }

        [TestMethod]
        public void AddInterestedViewModelInstance_ViewModelForNotExistingInterestedInViewModel()
        {
            var manager = new ViewModelManager();

            var interestedViewModel = new InterestedViewModel();

            manager.AddInterestedViewModelInstance(typeof(InterestingViewModel), interestedViewModel);
        }

        [TestMethod]
        public void RemoveInterestedViewModelInstance_FirstArgumentNull()
        {
            var manager = new ViewModelManager();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => manager.RemoveInterestedViewModelInstance(null, null));
        }

        [TestMethod]
        public void RemoveInterestedViewModelInstance_SecondArgumentNull()
        {
            var manager = new ViewModelManager();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => manager.RemoveInterestedViewModelInstance(typeof(InterestingViewModel), null));
        }

        [TestMethod]
        public void RemoveInterestedViewModelInstance_ViewModelForExistingInterestedInViewModel()
        {
            var manager = new ViewModelManager();

            var interestingViewModel = new InterestingViewModel();
            var interestedViewModel = new InterestedViewModel();

            manager.RegisterViewModelInstance(interestingViewModel);
            manager.AddInterestedViewModelInstance(typeof(InterestingViewModel), interestedViewModel);
            manager.RemoveInterestedViewModelInstance(typeof(InterestingViewModel), interestedViewModel);
        }

        [TestMethod]
        public void RemoveInterestedViewModelInstance_ViewModelForNotExistingInterestedInViewModel()
        {
            var manager = new ViewModelManager();

            var interestedViewModel = new InterestedViewModel();

            manager.AddInterestedViewModelInstance(typeof(InterestingViewModel), interestedViewModel);
            manager.RemoveInterestedViewModelInstance(typeof(InterestingViewModel), interestedViewModel);
        }

        [TestMethod]
        public void RemoveInterestedViewModelInstance_ViewModelForNotRegisteredInterestedInViewModel()
        {
            var manager = new ViewModelManager();

            var interestedViewModel = new InterestedViewModel();

            manager.RemoveInterestedViewModelInstance(typeof(InterestingViewModel), interestedViewModel);
        }
    }
}