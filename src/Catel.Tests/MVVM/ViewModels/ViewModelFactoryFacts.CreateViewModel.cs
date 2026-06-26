namespace Catel.Tests.MVVM.ViewModels;

using Catel.MVVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using TestClasses;

public partial class ViewModelFactoryFacts
{
    [Test]
    public void ViewModelFactory_CreateViewModel_Returns_Null_For_Model_Injection_Only_View_Model_Without_Data_Context()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModelFactory = new ViewModelFactory(new NullLogger<ViewModelFactory>(), serviceProvider);
        var viewModel = viewModelFactory.CreateViewModel(typeof(ViewModelFactoryModelInjectionOnlyTestViewModel), dataContext: null);

        Assert.That(viewModel, Is.Null);
    }

    [Test]
    public void ViewModelFactory_CreateViewModel_Creates_View_Model_Using_Dependency_Injection_When_First_Constructor_Parameter_Is_Registered()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModelFactory = new ViewModelFactory(new NullLogger<ViewModelFactory>(), serviceProvider);
        var viewModel = viewModelFactory.CreateViewModel(typeof(ViewModelFactoryTestViewModelWithOnlyDefaultConstructor), dataContext: null);

        Assert.That(viewModel, Is.TypeOf<ViewModelFactoryTestViewModelWithOnlyDefaultConstructor>());
    }

    [Test]
    public void ViewModelFactory_CreateViewModel_Injects_Array_As_Single_Model_When_Constructor_Accepts_Array_Type()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModelFactory = new ViewModelFactory(new NullLogger<ViewModelFactory>(), serviceProvider);
        var items = new[] { "a", "b", "c" };
        var viewModel = viewModelFactory.CreateViewModel(typeof(ViewModelFactoryArrayModelTestViewModel), dataContext: items) as ViewModelFactoryArrayModelTestViewModel;

        Assert.That(viewModel, Is.Not.Null);
        Assert.That(viewModel!.Items, Is.SameAs(items));
    }

    [Test]
    public void ViewModelFactory_CreateViewModel_Injects_Array_As_Single_Model_When_Constructor_Accepts_IReadOnlyList()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModelFactory = new ViewModelFactory(new NullLogger<ViewModelFactory>(), serviceProvider);
        var items = new[] { "a", "b", "c" };
        var viewModel = viewModelFactory.CreateViewModel(typeof(ViewModelFactoryReadOnlyListModelTestViewModel), dataContext: items) as ViewModelFactoryReadOnlyListModelTestViewModel;

        Assert.That(viewModel, Is.Not.Null);
        Assert.That(viewModel!.Items, Is.SameAs(items));
    }

    [Test]
    public void ViewModelFactory_CreateViewModel_Injects_Array_As_Single_Model_When_Constructor_Accepts_IEnumerable()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModelFactory = new ViewModelFactory(new NullLogger<ViewModelFactory>(), serviceProvider);
        var items = new[] { "a", "b", "c" };
        var viewModel = viewModelFactory.CreateViewModel(typeof(ViewModelFactoryEnumerableModelTestViewModel), dataContext: items) as ViewModelFactoryEnumerableModelTestViewModel;

        Assert.That(viewModel, Is.Not.Null);
        Assert.That(viewModel!.Items, Is.SameAs(items));
    }

    [Test]
    public void ViewModelFactory_CreateViewModel_Spreads_Object_Array_As_Multiple_Arguments_When_No_Constructor_Accepts_Array_Type()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModelFactory = new ViewModelFactory(new NullLogger<ViewModelFactory>(), serviceProvider);

        // ViewModelFactoryTestViewModel has a constructor accepting (int, IServiceProvider), so passing
        // new object[] { 42 } should still spread to a single int argument.
        var viewModel = viewModelFactory.CreateViewModel(typeof(ViewModelFactoryTestViewModel), dataContext: new object[] { 42 }) as ViewModelFactoryTestViewModel;

        Assert.That(viewModel, Is.Not.Null);
        Assert.That(viewModel!.Integer, Is.EqualTo(42));
    }
}
