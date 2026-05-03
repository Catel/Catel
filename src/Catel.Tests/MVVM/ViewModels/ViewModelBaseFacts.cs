namespace Catel.Tests.MVVM.ViewModels;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Catel.Data;
using Catel.MVVM;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestFeaturedViewModel = TestClasses.TestFeaturedViewModel;

[TestFixture]
public partial class ViewModelBaseFacts
{
    [TestCase]
    public async Task Is_Not_Dirty_At_Startup()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var vm = new TestFeaturedViewModel(serviceProvider);

        Assert.That(vm.IsDirty, Is.False);
    }

    [TestCase]
    public async Task Protect_Properties_After_Closing()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var vm = new TestFeaturedViewModel(serviceProvider);
        var freezable = (IFreezable)vm;

        await vm.InitializeViewModelAsync();

        vm.FirstName = "John";
        Assert.That(vm.FirstName, Is.EqualTo("John"));
        Assert.That(freezable.IsFrozen, Is.False);

        await vm.SaveAndCloseViewModelAsync();

        vm.FirstName = "Jane";
        Assert.That(vm.FirstName, Is.EqualTo("John"));
        Assert.That(freezable.IsFrozen, Is.True);

        await vm.InitializeViewModelAsync();

        vm.FirstName = "Jane";
        Assert.That(vm.FirstName, Is.EqualTo("Jane"));
        Assert.That(freezable.IsFrozen, Is.False);
    }

    [TestCase]
    public async Task Cancel_After_Close_Protection()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var vm = new TestFeaturedViewModel(serviceProvider);

        var canceledCalled = false;
        var closedCalled = false;

        vm.CanceledAsync += (_, _) => { canceledCalled = true; return Task.CompletedTask; };
        vm.ClosedAsync += (_, _) => { closedCalled = true; return Task.CompletedTask; };

        Assert.That(canceledCalled, Is.EqualTo(false));
        Assert.That(closedCalled, Is.EqualTo(false));

        await vm.CancelAndCloseViewModelAsync();

        Assert.That(canceledCalled, Is.EqualTo(true));
        Assert.That(closedCalled, Is.EqualTo(true));

        canceledCalled = false;
        closedCalled = false;

        await vm.CancelAndCloseViewModelAsync();

        Assert.That(canceledCalled, Is.EqualTo(false));
        Assert.That(closedCalled, Is.EqualTo(false));
    }

    [TestCase]
    public async Task Save_After_Close_Protection()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var vm = new TestFeaturedViewModel(serviceProvider);

        var savedCalled = false;
        var closedCalled = false;

        vm.SavedAsync += (_, _) => { savedCalled = true; return Task.CompletedTask; };
        vm.ClosedAsync += (_, _) => { closedCalled = true; return Task.CompletedTask; };

        Assert.That(savedCalled, Is.EqualTo(false));
        Assert.That(closedCalled, Is.EqualTo(false));

        await vm.SaveAndCloseViewModelAsync();

        Assert.That(savedCalled, Is.EqualTo(true));
        Assert.That(closedCalled, Is.EqualTo(true));

        savedCalled = false;
        closedCalled = false;

        await vm.SaveAndCloseViewModelAsync();

        Assert.That(savedCalled, Is.EqualTo(false));
        Assert.That(closedCalled, Is.EqualTo(false));
    }

    [TestCase]
    public async Task Close_After_Close_Protection()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var vm = new TestFeaturedViewModel(serviceProvider);

        var closedCalled = false;

        vm.ClosedAsync += (_, _) => { closedCalled = true; return Task.CompletedTask; };

        Assert.That(closedCalled, Is.EqualTo(false));

        await vm.CloseViewModelAsync(null);

        Assert.That(closedCalled, Is.EqualTo(true));

        closedCalled = false;

        await vm.CloseViewModelAsync(null);

        Assert.That(closedCalled, Is.EqualTo(false));
    }

    [Test]
    public async Task Multiple_ViewModels_Can_Be_Created_Concurrently()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        const int personsPerThread = 50;
        const int threadAmount = 10;
        var threads = new Thread[threadAmount];

        var allViewModels = new TestFeaturedViewModel[threadAmount][];

        for (var i = 0; i < threadAmount; i++)
        {
            threads[i] = new Thread(index =>
            {
                var localViewModels = new TestFeaturedViewModel[personsPerThread];
                for (var j = 0; j < personsPerThread; j++)
                {
                    localViewModels[j] = new TestFeaturedViewModel(serviceProvider);
                }

                lock (allViewModels)
                {
                    allViewModels[(int)index] = localViewModels;
                }
            });
        }

        for (var i = 0; i < threadAmount; i++)
        {
            threads[i].Start(i);
        }

        await Task.Delay(1000);

        for (var i = 0; i < threadAmount; i++)
        {
            threads[i].Join();
        }

        var flattenSortedIdentifiers =
            allViewModels
                .SelectMany(o => o)
                .Select(o => o.UniqueIdentifier)
                .OrderBy(o => o)
                .ToArray();

        var firstId = flattenSortedIdentifiers[0];

        Assert.That(flattenSortedIdentifiers, Is.EquivalentTo(Enumerable.Range(firstId, personsPerThread * threadAmount)));
    }

    [Test]
    public async Task Properties_Can_Be_Set_Concurrently_With_Object_Creation()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        const int personsPerThread = 50;
        const int threadAmount = 10;
        var threads = new Thread[threadAmount];

        var allViewModels = new TestFeaturedViewModel[threadAmount][];

        for (int i = 0; i < threadAmount; i++)
        {
            threads[i] = new Thread((index) =>
            {
                var localViewModels = new TestFeaturedViewModel[personsPerThread];

                for (int j = 0; j < personsPerThread; j++)
                {
                    var viewModel = new TestFeaturedViewModel(serviceProvider)
                    {
                        Age = 18
                    };

                    viewModel.Age = 19;

                    localViewModels[j] = viewModel;
                }

                allViewModels[(int)index] = localViewModels;
            });
        }

        for (int i = 0; i < threadAmount; i++)
        {
            threads[i].Start(i);
        }

        await Task.Delay(1000);

        for (int i = 0; i < threadAmount; i++)
        {
            threads[i].Join();
        }

        var flattenSortedIdentifiers =
            allViewModels
                .SelectMany(o => o)
                .Select(o => o.UniqueIdentifier)
                .OrderBy(o => o)
                .ToArray();

        var firstId = flattenSortedIdentifiers[0];

        Assert.That(flattenSortedIdentifiers, Is.EquivalentTo(Enumerable.Range(firstId, personsPerThread * threadAmount)));
    }

    [Test]
    public async Task Commands_Can_Be_Called_Concurrently_With_Object_Creation()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        const int personsPerThread = 50;
        const int threadAmount = 10;
        var threads = new Thread[threadAmount];

        var allViewModels = new TestFeaturedViewModel[threadAmount][];

        for (int i = 0; i < threadAmount; i++)
        {
            threads[i] = new Thread((index) =>
            {
                var localViewModels = new TestFeaturedViewModel[personsPerThread];

                for (int j = 0; j < personsPerThread; j++)
                {
                    var viewModel = new TestFeaturedViewModel(serviceProvider);
                    viewModel.GenerateData.Execute();
                    localViewModels[j] = viewModel;
                }

                allViewModels[(int)index] = localViewModels;
            });
        }

        for (int i = 0; i < threadAmount; i++)
        {
            threads[i].Start(i);
        }

        await Task.Delay(1000);

        for (int i = 0; i < threadAmount; i++)
        {
            threads[i].Join();
        }

        var flattenSortedIdentifiers =
            allViewModels
                .SelectMany(o => o)
                .Select(o => o.UniqueIdentifier)
                .OrderBy(o => o)
                .ToArray();

        var firstId = flattenSortedIdentifiers[0];

        Assert.That(flattenSortedIdentifiers, Is.EquivalentTo(Enumerable.Range(firstId, personsPerThread * threadAmount)));
    }
}
