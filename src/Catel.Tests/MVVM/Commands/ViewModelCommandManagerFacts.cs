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
            public void ThrowsArgumentNullExceptionForNullViewModel()
            {
                Assert.Throws<ArgumentNullException>(() => ViewModelCommandManager.Create(null));
            }

            [TestCase]
            public void ReturnsViewModelCommandManagerForViewModel()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddCatelCore();
                serviceCollection.AddCatelMvvm();

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
            public void ThrowsArgumentNullExceptionForNullHandler()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddCatelCore();
                serviceCollection.AddCatelMvvm();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var viewModel = new TestFeaturedViewModel(serviceProvider);
                var viewModelCommandManager = ViewModelCommandManager.Create(viewModel);

                Assert.Throws<ArgumentNullException>(() => viewModelCommandManager.AddHandler((Func<IViewModel, string, ICommand, object, Task>)null));
            }

            [TestCase]
            public async Task RegisteredHandlerGetsCalledAsync()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddCatelCore();
                serviceCollection.AddCatelMvvm();

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
