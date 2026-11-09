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

    [Test]
    public void ViewModelFactory_CreateViewModel_Allows_Parent_ViewModel_Injection_As_Model()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModelFactory = new ViewModelFactory(new NullLogger<ViewModelFactory>(), serviceProvider);
        var parentViewModel = viewModelFactory.CreateViewModel(typeof(ViewModelFactoryTestViewModelWithOnlyDefaultConstructor), dataContext: null) as IViewModel;

        Assert.That(parentViewModel, Is.Not.Null);

        // ViewModelFactoryParentViewModelTestViewModel accepts an IViewModel as its first constructor parameter
        var childViewModel = viewModelFactory.CreateViewModel(typeof(ViewModelFactoryParentViewModelTestViewModel), dataContext: parentViewModel) as ViewModelFactoryParentViewModelTestViewModel;

        Assert.That(childViewModel, Is.Not.Null);
        Assert.That(childViewModel!.ParentViewModel, Is.SameAs(parentViewModel));
    }

    [Test]
    public void ViewModelFactory_CreateViewModel_Falls_Back_To_Default_Constructor_When_Parent_ViewModel_Cannot_Be_Injected()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModelFactory = new ViewModelFactory(new NullLogger<ViewModelFactory>(), serviceProvider);
        var parentViewModel = viewModelFactory.CreateViewModel(typeof(ViewModelFactoryTestViewModelWithOnlyDefaultConstructor), dataContext: null) as IViewModel;

        Assert.That(parentViewModel, Is.Not.Null);

        // ViewModelFactoryModelInjectionOnlyTestViewModel requires a string model, not a parent ViewModel.
        // Since it cannot accept IViewModel as a model, the factory will try with empty args,
        // which fails the injection check, so it returns null.
        var childViewModel = viewModelFactory.CreateViewModel(typeof(ViewModelFactoryModelInjectionOnlyTestViewModel), dataContext: parentViewModel) as ViewModelFactoryModelInjectionOnlyTestViewModel;

        Assert.That(childViewModel, Is.Null);
    }

    [Test]
    public void ViewModelFactory_CreateViewModel_Prefers_Parent_ViewModel_Injection_Over_Empty_Constructor()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModelFactory = new ViewModelFactory(new NullLogger<ViewModelFactory>(), serviceProvider);
        var parentViewModel = viewModelFactory.CreateViewModel(typeof(ViewModelFactoryTestViewModelWithOnlyDefaultConstructor), dataContext: null) as IViewModel;

        Assert.That(parentViewModel, Is.Not.Null);

        // ViewModelFactoryParentViewModelTestViewModel has both:
        // - A constructor accepting (IServiceProvider) - the default/empty constructor
        // - A constructor accepting (IViewModel, IServiceProvider) - parent ViewModel injection
        // When a parent ViewModel is passed, it should use the constructor that accepts it (parent injection preferred)
        var childViewModel = viewModelFactory.CreateViewModel(typeof(ViewModelFactoryParentViewModelTestViewModel), dataContext: parentViewModel) as ViewModelFactoryParentViewModelTestViewModel;

        Assert.That(childViewModel, Is.Not.Null);
        Assert.That(childViewModel!.ParentViewModel, Is.SameAs(parentViewModel));
    }
}
