namespace Catel.Tests.MVVM
{
    using System;
    using Catel.MVVM;
    using ViewModels.TestClasses;
    using NUnit.Framework;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public class ViewModelCommandManagerFacts
    {
        [TestFixture]
        public class TheCreateMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullViewModel()
            {
                Assert.Throws<ArgumentNullException>(() => new ViewModelCommandManager(null, null));
            }

            [TestCase]
            public void ReturnsViewModelCommandManagerForViewModel()
            {
                var viewModel = new TestViewModel();
                var viewModelCommandManager = ViewModelCommandManager.Create(viewModel);

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var viewModel = new TestFeaturedViewModel(serviceProvider);
                var viewModelCommandManager = new ViewModelCommandManager(viewModel, serviceProvider);

                Assert.That(viewModelCommandManager, Is.Not.Null);
            }
        }

        [TestFixture]
        public class TheAddHandlerMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullHandler()
            {
                var viewModel = new TestViewModel();
                var viewModelCommandManager = ViewModelCommandManager.Create(viewModel);

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var viewModel = new TestFeaturedViewModel(serviceProvider);
                var viewModelCommandManager = new ViewModelCommandManager(viewModel, serviceProvider);

                Assert.Throws<ArgumentNullException>(() => viewModelCommandManager.AddHandler((Func<IViewModel, string, ICommand, object, Task>)null));
            }

            [TestCase]
            public async Task RegisteredHandlerGetsCalledAsync()
            {
                var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var viewModel = new TestFeaturedViewModel(serviceProvider);
                var viewModelCommandManager = new ViewModelCommandManager(viewModel, serviceProvider);
                await viewModel.InitializeViewModelAsync();

                var called = false;

                viewModelCommandManager.AddHandler(async (vm, property, command, commandParameter) => called = true);
                viewModel.GenerateData.Execute();

                Assert.That(called, Is.True);
            }
        }
    }
}