namespace Catel.Tests.MVVM.ViewModels;

using System.Threading;
using System.Threading.Tasks;
using Catel.MVVM;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestClasses;

public partial class ViewModelBaseFacts
{
    [TestCase, RequiresThread(ApartmentState.STA)]
    public async Task InvalidateCommands_Manual()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        bool canExecuteChangedTriggered = false;

        var viewModel = new TestFeaturedViewModel(serviceProvider);
        viewModel.SetInvalidateCommandsOnPropertyChanged(false);

        ICatelCommand command = viewModel.GenerateData;
        command.CanExecuteChanged += delegate
        {
            canExecuteChangedTriggered = true;
        };

        // By default, command can be executed
        Assert.That(viewModel.GenerateData.CanExecute(null), Is.True);

        viewModel.FirstName = "first name";

        Assert.That(viewModel.GenerateData.CanExecute(null), Is.False);

        await Task.Delay(100);

        Assert.That(canExecuteChangedTriggered, Is.False);
    }

    [TestCase, RequiresThread(ApartmentState.STA)]
    public async Task InvalidateCommands_AutomaticByPropertyChange()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        bool canExecuteChangedTriggered = false;

        var viewModel = new TestFeaturedViewModel(new Person(), serviceProvider);
        viewModel.SetInvalidateCommandsOnPropertyChanged(true);
        await viewModel.InitializeViewModelAsync();

        ICatelCommand command = viewModel.GenerateData;

        command.CanExecuteChanged += delegate
        {
            canExecuteChangedTriggered = true;
        };

        // By default, command can be executed
        Assert.That(viewModel.GenerateData.CanExecute(null), Is.True);

        Assert.That(viewModel.FirstName, Is.Not.EqualTo("first name"));
        viewModel.FirstName = "first name";

        Assert.That(viewModel.GenerateData.CanExecute(null), Is.False);

        await Task.Delay(100);

        Assert.That(canExecuteChangedTriggered, Is.True);
    }

    [TestCase, RequiresThread(ApartmentState.STA)]
    public async Task InvalidateCommands_AutomaticByPropertyChange_AfterInitialization()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        bool canExecuteChangedTriggered = false;

        var viewModel = new TestFeaturedViewModel(new Person(), serviceProvider);
        viewModel.SetInvalidateCommandsOnPropertyChanged(true);

        ICatelCommand command = viewModel.GenerateData;

        command.CanExecuteChanged += delegate
        {
            canExecuteChangedTriggered = true;
        };

        Assert.That(viewModel.GenerateData.CanExecute(null), Is.True);

        viewModel.FirstName = "first name";
        Assert.That(viewModel.GenerateData.CanExecute(null), Is.False);

        await Task.Delay(100);

        Assert.That(canExecuteChangedTriggered, Is.False);

        await viewModel.InitializeViewModelAsync();
        await Task.Delay(100);

        Assert.That(canExecuteChangedTriggered, Is.True);
    }

    [TestCase, RequiresThread(ApartmentState.STA)]
    public async Task DeferredInvalidation_RespectsDisabledState_WhenDisabledBeforeInitialization()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        bool canExecuteChangedTriggered = false;

        var viewModel = new TestFeaturedViewModel(new Person(), serviceProvider);
        viewModel.SetInvalidateCommandsOnPropertyChanged(true);

        ICatelCommand command = viewModel.GenerateData;

        command.CanExecuteChanged += delegate
        {
            canExecuteChangedTriggered = true;
        };

        Assert.That(viewModel.GenerateData.CanExecute(null), Is.True);

        viewModel.FirstName = "first name";
        Assert.That(viewModel.GenerateData.CanExecute(null), Is.False);

        Assert.That(canExecuteChangedTriggered, Is.False);

        viewModel.SetInvalidateCommandsOnPropertyChanged(false);

        await viewModel.InitializeViewModelAsync();

        Assert.That(canExecuteChangedTriggered, Is.True);
    }
}
