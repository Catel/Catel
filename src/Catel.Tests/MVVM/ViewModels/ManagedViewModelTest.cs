namespace Catel.Tests.MVVM.ViewModels
{
    using System;
    using Catel.MVVM;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestClasses;

    [TestFixture]
    public class ManagedViewModelTest
    {
        [TestCase]
        public void Constructor()
        {
            var viewModel = new ManagedViewModel(typeof(TestFeaturedViewModel));
            Assert.That(viewModel.ViewModelType, Is.EqualTo(typeof(TestFeaturedViewModel)));
        }

        [TestCase]
        public void AddViewModelInstance_Null()
        {
            var viewModel = new ManagedViewModel(typeof(TestFeaturedViewModel));

            Assert.Throws<ArgumentNullException>(() => viewModel.AddViewModelInstance(null));
        }

        [TestCase]
        public void AddViewModelInstance_WrongType()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var viewModel = new ManagedViewModel(typeof(TestFeaturedViewModel));

            try
            {
                viewModel.AddViewModelInstance(new TestViewModelWithDeferredValidation(serviceProvider));

                Assert.Fail("Expected WrongViewModelTypeException");
            }
            catch (WrongViewModelTypeException ex)
            {
                Assert.That(ex.ActualType, Is.EqualTo(typeof(TestViewModelWithDeferredValidation)));
                Assert.That(ex.ExpectedType, Is.EqualTo(typeof(TestFeaturedViewModel)));
            }
        }

        [TestCase]
        public void AddViewModelInstance_NewInstance()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var viewModel = new ManagedViewModel(typeof(TestFeaturedViewModel));
            viewModel.AddViewModelInstance(new TestFeaturedViewModel(serviceProvider));
        }

        [TestCase]
        public void RemoveViewModelInstance_Null()
        {
            var viewModel = new ManagedViewModel(typeof(TestFeaturedViewModel));

            Assert.Throws<ArgumentNullException>(() => viewModel.RemoveViewModelInstance(null));
        }

        [TestCase]
        public void RemoveViewModelInstance_NotRegisteredViewModel()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var viewModel = new ManagedViewModel(typeof(TestFeaturedViewModel));
            viewModel.RemoveViewModelInstance(new TestFeaturedViewModel(serviceProvider));
        }

        [TestCase]
        public void RemoveViewModelInstance_RegisteredViewModel()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var viewModel = new ManagedViewModel(typeof(TestFeaturedViewModel));

            var interestingViewModel = new TestFeaturedViewModel(serviceProvider);
            viewModel.AddViewModelInstance(interestingViewModel);
            viewModel.RemoveViewModelInstance(interestingViewModel);
        }
    }
}
