namespace Catel.Tests.MVVM.ViewModels
{
    using System;
    using System.Linq;
    using Catel.MVVM;
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
                using (var vmManager = new ViewModelManager())
                {
                    Assert.Throws<ArgumentNullException>(() => vmManager.RegisterModel(null, model));
                }
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullModel()
            {
                var model = new Person();
                var vm = new TestViewModel(model);
                using (var vmManager = new ViewModelManager())
                {
                    Assert.Throws<ArgumentNullException>(() => vmManager.RegisterModel(vm, null));
                }
            }

            [TestCase]
            public void RegistersModelForViewModel()
            {
                var model = new Person();
                var vm = new TestViewModel(model);
                using (var vmManager = new ViewModelManager())
                {
                    vmManager.RegisterViewModelInstance(vm);
                    vmManager.RegisterModel(vm, model);

                    var foundVm = vmManager.GetViewModelsOfModel(model).First();

                    Assert.That(foundVm, Is.EqualTo(vm));
                }
            }
        }

        [TestFixture]
        public class TheUnregisterModelMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullViewModel()
            {
                var model = new Person();
                using (var vmManager = new ViewModelManager())
                {
                    Assert.Throws<ArgumentNullException>(() => vmManager.UnregisterModel(null, model));
                }
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullModel()
            {
                var model = new Person();
                var vm = new TestViewModel(model);
                using (var vmManager = new ViewModelManager())
                {
                    Assert.Throws<ArgumentNullException>(() => vmManager.UnregisterModel(vm, null));
                }
            }

            [TestCase]
            public void UnregistersModelForViewModel()
            {
                var model = new Person();
                var vm = new TestViewModel(model);
                using (var vmManager = new ViewModelManager())
                {
                    vmManager.RegisterViewModelInstance(vm);
                    vmManager.RegisterModel(vm, model);

                    var foundVm = vmManager.GetViewModelsOfModel(model).First();

                    Assert.That(foundVm, Is.EqualTo(vm));

                    vmManager.UnregisterModel(vm, model);

                    foundVm = vmManager.GetViewModelsOfModel(model).FirstOrDefault();

                    Assert.That(foundVm, Is.Null);
                }
            }
        }

        [TestFixture]
        public class TheUnregisterAllModelsMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullViewModel()
            {
                using (var vmManager = new ViewModelManager())
                {
                    Assert.Throws<ArgumentNullException>(() => vmManager.UnregisterAllModels(null));
                }
            }

            [TestCase]
            public void UnregistersAllModelForViewModel()
            {
                var model = new Person();
                var vm = new TestViewModel(model);
                using (var vmManager = new ViewModelManager())
                {
                    vmManager.RegisterViewModelInstance(vm);
                    vmManager.RegisterModel(vm, model);

                    var foundVm = vmManager.GetViewModelsOfModel(model).First();

                    Assert.That(foundVm, Is.EqualTo(vm));

                    vmManager.UnregisterAllModels(vm);

                    foundVm = vmManager.GetViewModelsOfModel(model).FirstOrDefault();

                    Assert.That(foundVm, Is.Null);
                }
            }
        }

        [TestFixture]
        public class TheGetViewModelsOfModelMethod
        {
            [TestCase]
            public void ReturnsNullForUnregisteredModel()
            {
                var model = new Person();
                using (var vmManager = new ViewModelManager())
                {
                    var foundVm = vmManager.GetViewModelsOfModel(model).FirstOrDefault();

                    Assert.That(foundVm, Is.Null);
                }
            }

            [TestCase]
            public void ReturnsViewModelOfRegisteredModel()
            {
                var model = new Person();
                var vm = new TestViewModel(model);
                using (var vmManager = new ViewModelManager())
                {
                    vmManager.RegisterViewModelInstance(vm);
                    vmManager.RegisterModel(vm, model);

                    var foundVm = vmManager.GetViewModelsOfModel(model).First();

                    Assert.That(foundVm, Is.EqualTo(vm));
                }
            }
        }

        [TestFixture]
        public class TheGetViewModelMethod
        {
            [TestCase]
            public void ReturnsNullForUnregisteredViewModel()
            {
                using (var vmManager = new ViewModelManager())
                {
                    var foundvm = vmManager.GetViewModel(42);

                    Assert.That(foundvm, Is.Null);
                }
            }

            [TestCase]
            public void ReturnsViewModelForRegisteredViewModel()
            {
                var vm = new TestViewModel();
                using (var vmManager = new ViewModelManager())
                {
                    vmManager.RegisterViewModelInstance(vm);
                    var foundvm = vmManager.GetViewModel(vm.UniqueIdentifier);

                    Assert.That(foundvm, Is.EqualTo(vm));
                }
            }
        }

        [TestFixture]
        public class TheGetChildViewModelsMethod
        {
            [TestCase]
            public void ReturnsNullForUnregisteredChildViewModels()
            {
                using (var viewModelManager = new ViewModelManager())
                {
                    var foundViewModels = viewModelManager.GetChildViewModels(42);

                    Assert.That(foundViewModels.Count(), Is.EqualTo(0));
                }
            }

            [TestCase]
            public void ReturnsChildViewModelsUsingParentInstance()
            {
                var parentViewModel = new TestViewModel() as IRelationalViewModel;
                var childViewModel = new TestViewModel() as IRelationalViewModel;
                using (var viewModelManager = new ViewModelManager())
                {
                    parentViewModel.RegisterChildViewModel(childViewModel as IViewModel);
                    childViewModel.SetParentViewModel(parentViewModel as IViewModel);

                    viewModelManager.RegisterViewModelInstance(parentViewModel as IViewModel);
                    viewModelManager.RegisterViewModelInstance(childViewModel as IViewModel);

                    var foundViewModels = viewModelManager.GetChildViewModels(parentViewModel as IViewModel);

                    Assert.IsNotNull(foundViewModels);
                    Assert.That(foundViewModels.Contains(childViewModel), Is.True);
                }
            }
        }

        [TestFixture]
        public class TheGetFirstOrDefaultInstanceMethod
        {
            [TestCase]
            public void ReturnsNullForUnregisteredViewModel()
            {
                using (var vmManager = new ViewModelManager())
                {
                    var foundvm = vmManager.GetFirstOrDefaultInstance<TestViewModel>();

                    Assert.That(foundvm, Is.Null);
                }
            }

            [TestCase]
            public void ReturnsViewModelForRegisteredViewModel()
            {
                var vm = new TestViewModel();
                using (var vmManager = new ViewModelManager())
                {
                    vmManager.RegisterViewModelInstance(vm);
                    var foundvm = vmManager.GetFirstOrDefaultInstance<TestViewModel>();

                    Assert.That(foundvm, Is.EqualTo(vm));
                }
            }

            [TestCase]
            public void ReturnsViewModelForMultiRegisteredViewModel()
            {
                var firstvm = new TestViewModel()
                {
                    FirstName = "John",
                    LastName = "Doe"
                };

                var secondvm = new TestViewModel();

                using (var vmManager = new ViewModelManager())
                {
                    vmManager.RegisterViewModelInstance(firstvm);
                    vmManager.RegisterViewModelInstance(secondvm);
                    var foundvm = vmManager.GetFirstOrDefaultInstance<TestViewModel>();

                    Assert.That(foundvm, Is.EqualTo(firstvm));
                }
            }

            [TestCase]
            public void ShouldFailsDueToANonIViewModelType()
            {
                using (var viewModelManager = new ViewModelManager())
                {
                    Assert.Throws<ArgumentException>(() => viewModelManager.GetFirstOrDefaultInstance(typeof(Type)));
                }
            }
        }

        [TestFixture]
        public class TheActiveViewModelsMethod
        {
            [TestCase]
            public void MustBeNotNullAfterConstructed()
            {
                using (var vmManager = new ViewModelManager())
                {
                    Assert.IsNotNull(vmManager.ActiveViewModels);
                }
            }

            [TestCase]
            public void MustExistsForRegisteredViewModels()
            {
                var firstvm = new TestViewModel();
                var secondvm = new TestViewModel();

                using (var vmManager = new ViewModelManager())
                {
                    vmManager.RegisterViewModelInstance(firstvm);
                    vmManager.RegisterViewModelInstance(secondvm);

                    var vmList = vmManager.ActiveViewModels.ToList();

                    Assert.That(vmList.Any(vm => TagHelper.AreTagsEqual(vm.UniqueIdentifier, firstvm.UniqueIdentifier)), Is.True);
                    Assert.That(vmList.Any(vm => TagHelper.AreTagsEqual(vm.UniqueIdentifier, secondvm.UniqueIdentifier)), Is.True);
                }
            }
        }

        [TestCase]
        public void RegisterViewModelInstance_Null()
        {
            using (var manager = new ViewModelManager())
            {
                Assert.Throws<ArgumentNullException>(() => manager.RegisterViewModelInstance(null));
            }
        }

        [TestCase]
        public void RegisterViewModelInstance_ViewModel()
        {
            using (var manager = new ViewModelManager())
            {
                manager.RegisterViewModelInstance(new TestViewModel());
            }
        }

        [TestCase]
        public void UnregisterViewModelInstance_Null()
        {
            using (var manager = new ViewModelManager())
            {
                Assert.Throws<ArgumentNullException>(() => manager.UnregisterViewModelInstance(null));
            }
        }

        [TestCase]
        public void UnregisterViewModelInstance_ExistingViewModel()
        {
            using (var manager = new ViewModelManager())
            {
                var viewModel = new TestViewModel();

                manager.RegisterViewModelInstance(viewModel);
                manager.UnregisterViewModelInstance(viewModel);
            }
        }

        [TestCase]
        public void UnregisterViewModelInstance_NotExistingViewModel()
        {
            using (var manager = new ViewModelManager())
            {
                manager.UnregisterViewModelInstance(new TestViewModel());
            }
        }
    }
}
