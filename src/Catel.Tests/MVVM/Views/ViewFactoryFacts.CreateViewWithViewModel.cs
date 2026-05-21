namespace Catel.Tests.MVVM.Views;

using System.Threading;
using System.Windows;
using Catel.MVVM;
using Catel.MVVM.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

public class ViewFactoryFacts
{
    [Test]
    [Apartment(ApartmentState.STA)]
    public void CreateViewWithViewModel_UsesDefaultConstructor_WhenNoViewModelConstructorExists()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModelLocatorMock = new Mock<IViewModelLocator>();
        var viewFactory = new ViewFactory(NullLogger<ViewFactory>.Instance, serviceProvider, viewModelLocatorMock.Object);

        var viewModel = new Mock<IViewModel>().Object;
        var view = viewFactory.CreateViewWithViewModel(typeof(ViewWithObjectAndDefaultConstructors), viewModel) as ViewWithObjectAndDefaultConstructors;

        Assert.That(view, Is.Not.Null);
        Assert.That(view!.DefaultConstructorCalled, Is.True);
        Assert.That(view.ObjectConstructorCalled, Is.False);
        Assert.That(view.DataContext, Is.SameAs(viewModel));
    }

    [Test]
    [Apartment(ApartmentState.STA)]
    public void CreateViewWithViewModel_UsesViewModelConstructor_WhenAvailable()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var viewModelLocatorMock = new Mock<IViewModelLocator>();
        var viewFactory = new ViewFactory(NullLogger<ViewFactory>.Instance, serviceProvider, viewModelLocatorMock.Object);

        var viewModel = new Mock<IViewModel>().Object;
        var view = viewFactory.CreateViewWithViewModel(typeof(ViewWithIViewModelAndDefaultConstructors), viewModel) as ViewWithIViewModelAndDefaultConstructors;

        Assert.That(view, Is.Not.Null);
        Assert.That(view!.ViewModelConstructorCalled, Is.True);
        Assert.That(view.DefaultConstructorCalled, Is.False);
        Assert.That(view.DataContext, Is.SameAs(viewModel));
    }

    private class ViewWithObjectAndDefaultConstructors : FrameworkElement
    {
        public ViewWithObjectAndDefaultConstructors()
        {
            DefaultConstructorCalled = true;
        }

        public ViewWithObjectAndDefaultConstructors(object dataContext)
        {
            ObjectConstructorCalled = true;
            DataContext = dataContext;
        }

        public bool DefaultConstructorCalled { get; }

        public bool ObjectConstructorCalled { get; }
    }

    private class ViewWithIViewModelAndDefaultConstructors : FrameworkElement
    {
        public ViewWithIViewModelAndDefaultConstructors()
        {
            DefaultConstructorCalled = true;
        }

        public ViewWithIViewModelAndDefaultConstructors(IViewModel dataContext)
        {
            ViewModelConstructorCalled = true;
            DataContext = dataContext;
        }

        public bool DefaultConstructorCalled { get; }

        public bool ViewModelConstructorCalled { get; }
    }
}
