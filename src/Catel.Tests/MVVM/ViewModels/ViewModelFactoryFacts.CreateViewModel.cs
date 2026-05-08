namespace Catel.Tests.MVVM.ViewModels;

using Catel.MVVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using TestClasses;

public partial class ViewModelFactoryFacts
{
    [TestCase]
    public void ViewModelFactory_CreateViewModel_Returns_Null_For_Model_Injection_Only_View_Model_Without_Data_Context()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModelFactory = new ViewModelFactory(new NullLogger<ViewModelFactory>(), serviceProvider);
        var viewModel = viewModelFactory.CreateViewModel(typeof(ViewModelFactoryModelInjectionOnlyTestViewModel), dataContext: null);

        Assert.That(viewModel, Is.Null);
    }

    [TestCase]
    public void ViewModelFactory_CreateViewModel_Creates_View_Model_Using_Dependency_Injection_When_First_Constructor_Parameter_Is_Registered()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModelFactory = new ViewModelFactory(new NullLogger<ViewModelFactory>(), serviceProvider);
        var viewModel = viewModelFactory.CreateViewModel(typeof(ViewModelFactoryTestViewModelWithOnlyDefaultConstructor), dataContext: null);

        Assert.That(viewModel, Is.TypeOf<ViewModelFactoryTestViewModelWithOnlyDefaultConstructor>());
    }
}
