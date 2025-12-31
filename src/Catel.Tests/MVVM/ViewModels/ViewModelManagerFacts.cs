namespace Catel.Tests.MVVM.ViewModels
{
    using System;
    using System.Linq;
    using Catel.MVVM;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging.Abstractions;
    using NUnit.Framework;
    using TestClasses;

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

                using (var vmManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
                {
                    Assert.Throws<ArgumentNullException>(() => vmManager.RegisterModel(null, model));
                }
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullModel()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddCatelCore();
                serviceCollection.AddCatelMvvm();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var model = new Person();
                var vm = new TestFeaturedViewModel(model, serviceProvider);
                using (var vmManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
                {
                    Assert.Throws<ArgumentNullException>(() => vmManager.RegisterModel(vm, null));
                }
            }

            [TestCase]
            public void RegistersModelForViewModel()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddCatelCore();
                serviceCollection.AddCatelMvvm();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var model = new Person();
                var vm = new TestFeaturedViewModel(model, serviceProvider);
                using (var vmManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
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
                using (var vmManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
                {
                    Assert.Throws<ArgumentNullException>(() => vmManager.UnregisterModel(null, model));
                }
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullModel()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddCatelCore();
                serviceCollection.AddCatelMvvm();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var model = new Person();
                var vm = new TestFeaturedViewModel(model, serviceProvider);
                using (var vmManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
                {
                    Assert.Throws<ArgumentNullException>(() => vmManager.UnregisterModel(vm, null));
                }
            }

            [TestCase]
            public void UnregistersModelForViewModel()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddCatelCore();
                serviceCollection.AddCatelMvvm();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var model = new Person();
                var vm = new TestFeaturedViewModel(model, serviceProvider);
                using (var vmManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
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
                using (var vmManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
                {
                    Assert.Throws<ArgumentNullException>(() => vmManager.UnregisterAllModels(null));
                }
            }

            [TestCase]
            public void UnregistersAllModelForViewModel()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddCatelCore();
                serviceCollection.AddCatelMvvm();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var model = new Person();
                var vm = new TestFeaturedViewModel(model, serviceProvider);
                using (var vmManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
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
                using (var vmManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
                {
                    var foundVm = vmManager.GetViewModelsOfModel(model).FirstOrDefault();

                    Assert.That(foundVm, Is.Null);
                }
            }

            [TestCase]
            public void ReturnsViewModelOfRegisteredModel()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddCatelCore();
                serviceCollection.AddCatelMvvm();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var model = new Person();
                var vm = new TestFeaturedViewModel(model, serviceProvider);
                using (var vmManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
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
                using (var vmManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
                {
                    var foundvm = vmManager.GetViewModel(42);

                    Assert.That(foundvm, Is.Null);
                }
            }

            [TestCase]
            public void ReturnsViewModelForRegisteredViewModel()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddCatelCore();
                serviceCollection.AddCatelMvvm();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var vm = new TestFeaturedViewModel(serviceProvider);
                using (var vmManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
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
                using (var viewModelManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
                {
                    var foundViewModels = viewModelManager.GetChildViewModels(42);

                    Assert.That(foundViewModels.Count(), Is.EqualTo(0));
                }
            }

            [TestCase]
            public void ReturnsChildViewModelsUsingParentInstance()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddCatelCore();
                serviceCollection.AddCatelMvvm();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var parentViewModel = new TestFeaturedViewModel(serviceProvider) as IRelationalViewModel;
                var childViewModel = new TestFeaturedViewModel(serviceProvider) as IRelationalViewModel;
                using (var viewModelManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
                {
                    parentViewModel.RegisterChildViewModel(childViewModel as IViewModel);
                    childViewModel.SetParentViewModel(parentViewModel as IViewModel);

                    viewModelManager.RegisterViewModelInstance(parentViewModel as IViewModel);
                    viewModelManager.RegisterViewModelInstance(childViewModel as IViewModel);

                    var foundViewModels = viewModelManager.GetChildViewModels(parentViewModel as IViewModel);

                    Assert.That(foundViewModels, Is.Not.Null);
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
                using (var vmManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
                {
                    var foundvm = vmManager.GetFirstOrDefaultInstance<TestFeaturedViewModel>();

                    Assert.That(foundvm, Is.Null);
                }
            }

            [TestCase]
            public void ReturnsViewModelForRegisteredViewModel()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddCatelCore();
                serviceCollection.AddCatelMvvm();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var vm = new TestFeaturedViewModel(serviceProvider);
                using (var vmManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
                {
                    vmManager.RegisterViewModelInstance(vm);
                    var foundvm = vmManager.GetFirstOrDefaultInstance<TestFeaturedViewModel>();

                    Assert.That(foundvm, Is.EqualTo(vm));
                }
            }

            [TestCase]
            public void ReturnsViewModelForMultiRegisteredViewModel()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddCatelCore();
                serviceCollection.AddCatelMvvm();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var firstvm = new TestFeaturedViewModel(serviceProvider)
                {
                    FirstName = "John",
                    LastName = "Doe"
                };

                var secondvm = new TestFeaturedViewModel(serviceProvider);

                using (var vmManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
                {
                    vmManager.RegisterViewModelInstance(firstvm);
                    vmManager.RegisterViewModelInstance(secondvm);
                    var foundvm = vmManager.GetFirstOrDefaultInstance<TestFeaturedViewModel>();

                    Assert.That(foundvm, Is.EqualTo(firstvm));
                }
            }

            [TestCase]
            public void ShouldFailsDueToANonIViewModelType()
            {
                using (var viewModelManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
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
                using (var vmManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
                {
                    Assert.That(vmManager.ActiveViewModels, Is.Not.Null);
                }
            }

            [TestCase]
            public void MustExistsForRegisteredViewModels()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddCatelCore();
                serviceCollection.AddCatelMvvm();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var firstvm = new TestFeaturedViewModel(serviceProvider);
                var secondvm = new TestFeaturedViewModel(serviceProvider);

                using (var vmManager = new ViewModelManager(new NullLogger<ViewModelManager>()))
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
            using (var manager = new ViewModelManager(new NullLogger<ViewModelManager>()))
            {
                Assert.Throws<ArgumentNullException>(() => manager.RegisterViewModelInstance(null));
            }
        }

        [TestCase]
        public void RegisterViewModelInstance_ViewModel()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();
            serviceCollection.AddCatelMvvm();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var manager = new ViewModelManager(new NullLogger<ViewModelManager>()))
            {
                manager.RegisterViewModelInstance(new TestFeaturedViewModel(serviceProvider));
            }
        }

        [TestCase]
        public void UnregisterViewModelInstance_Null()
        {
            using (var manager = new ViewModelManager(new NullLogger<ViewModelManager>()))
            {
                Assert.Throws<ArgumentNullException>(() => manager.UnregisterViewModelInstance(null));
            }
        }

        [TestCase]
        public void UnregisterViewModelInstance_ExistingViewModel()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();
            serviceCollection.AddCatelMvvm();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var manager = new ViewModelManager(new NullLogger<ViewModelManager>()))
            {
                var viewModel = new TestFeaturedViewModel(serviceProvider);

                manager.RegisterViewModelInstance(viewModel);
                manager.UnregisterViewModelInstance(viewModel);
            }
        }

        [TestCase]
        public void UnregisterViewModelInstance_NotExistingViewModel()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();
            serviceCollection.AddCatelMvvm();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var manager = new ViewModelManager(new NullLogger<ViewModelManager>()))
            {
                manager.UnregisterViewModelInstance(new TestFeaturedViewModel(serviceProvider));
            }
        }
    }
}
