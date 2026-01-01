namespace Catel.Tests.MVVM
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Catel.MVVM;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using ViewModels.TestClasses;

    public class ViewModelCommandManagerFacts
    {
        [TestFixture]
        public class TheCreateMethod
        {
            [TestCase]
            public void Throws_ArgumentNullException_For_Null_ViewModel()
            {
                Assert.Throws<ArgumentNullException>(() => ViewModelCommandManager.Create(null));
            }

            [TestCase]
            public void Returns_ViewModelCommandManager_For_ViewModel()
            {
                var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var viewModel = new TestFeaturedViewModel(serviceProvider);
                var viewModelCommandManager = ViewModelCommandManager.Create(viewModel);

                Assert.That(viewModelCommandManager, Is.Not.Null);
            }
        }

        [TestFixture]
        public class TheAddHandlerMethod
        {
            [TestCase]
            public void Throws_ArgumentNullException_For_Null_Handler()
            {
                var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var viewModel = new TestFeaturedViewModel(serviceProvider);
                var viewModelCommandManager = ViewModelCommandManager.Create(viewModel);

                Assert.Throws<ArgumentNullException>(() => viewModelCommandManager.AddHandler((Func<IViewModel, string, ICommand, object, Task>)null));
            }

            [TestCase]
            public async Task Registered_Handler_Gets_Called()
            {
                var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var viewModel = new TestFeaturedViewModel(serviceProvider);
                var viewModelCommandManager = ViewModelCommandManager.Create(viewModel);
                await viewModel.InitializeViewModelAsync();

                var called = false;

                viewModelCommandManager.AddHandler(async (vm, property, command, commandParameter) => called = true);
                viewModel.GenerateData.Execute();

                Assert.That(called, Is.True);
            }
        }
    }
}
