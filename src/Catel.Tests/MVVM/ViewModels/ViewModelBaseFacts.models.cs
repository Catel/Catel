namespace Catel.Tests.MVVM.ViewModels;

using System.Threading.Tasks;
using Catel.Data;
using Catel.MVVM;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestClasses;

public partial class ViewModelBaseFacts
{
    [TestCase]
    public void GetAllModels_With_Null()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var person = new Person();
        person.FirstName = "first_name";
        person.LastName = "last_name";

        var viewModel = new TestFeaturedViewModel(person, serviceProvider);

        var models = viewModel.GetAllModelsForTest();

        Assert.That(models.Length, Is.EqualTo(1));
        Assert.That(models[0], Is.EqualTo(person));
    }

    [TestCase]
    public void GetAllModels()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var person = new Person();
        person.FirstName = "first_name";
        person.LastName = "last_name";

        var viewModel = new TestFeaturedViewModel(person, serviceProvider);

        var specialValidationModel = new SpecialValidationModel();
        viewModel.SpecialValidationModel = specialValidationModel;

        var models = viewModel.GetAllModelsForTest();

        Assert.That(models.Length, Is.EqualTo(2));
        Assert.That(models[0], Is.EqualTo(person));
        Assert.That(models[1], Is.EqualTo(specialValidationModel));
    }

    [TestCase]
    public async Task ModelsSavedBySaveAsync()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var person = new Person();
        person.FirstName = "first name";
        person.LastName = "last name";

        var viewModel = new TestFeaturedViewModel(person, serviceProvider);
        Assert.That(person.IsInEditSession, Is.True);

        viewModel.FirstName = "new";

        await viewModel.SaveAndCloseViewModelAsync();

        Assert.That(person.IsInEditSession, Is.False);
        Assert.That(person.FirstName, Is.EqualTo("new"));
    }

    [TestCase]
    public async Task ModelsCanceledByCancelAsync()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var person = new Person();
        person.FirstName = "first name";
        person.LastName = "last name";

        var viewModel = new TestFeaturedViewModel(person, serviceProvider);
        Assert.That(person.IsInEditSession, Is.True);

        viewModel.FirstName = "new first name";

        await viewModel.CancelAndCloseViewModelAsync();

        Assert.That(person.IsInEditSession, Is.False);
        Assert.That(person.CalledCancelEdit, Is.True);
        Assert.That(person.CalledEndEdit, Is.False);
    }

    [TestCase]
    public async Task ModelsCanceledByCloseViewModelAsync_WhenNeitherSavedNorCanceled()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var person = new Person();
        person.FirstName = "first name";
        person.LastName = "last name";

        var viewModel = new TestFeaturedViewModel(person, serviceProvider);
        Assert.That(person.IsInEditSession, Is.True);

        await viewModel.CloseViewModelAsync(null);

        Assert.That(person.IsInEditSession, Is.False);
        Assert.That(person.CalledCancelEdit, Is.True);
        Assert.That(person.CalledEndEdit, Is.False);
    }

    /// <summary>
    /// Reproduces: Applying changes in a Dialog prevents further edits.
    /// Saving a ViewModel (e.g. via the Apply button) must keep the model in an active edit
    /// session so that subsequent saves and cancels still work correctly.
    /// </summary>
    [TestCase]
    public async Task ModelStillInEditSessionAfterSaveViewModelAsync()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var person = new Person();
        person.FirstName = "first name";
        person.LastName = "last name";

        var viewModel = new TestFeaturedViewModel(person, serviceProvider);
        Assert.That(person.IsInEditSession, Is.True);

        viewModel.FirstName = "applied first name";

        // Simulate clicking Apply (save without closing)
        var firstSaveResult = await viewModel.SaveViewModelAsync();

        Assert.That(firstSaveResult, Is.True);
        Assert.That(person.FirstName, Is.EqualTo("applied first name"));

        // After Apply the model must still be in an active edit session
        Assert.That(person.IsInEditSession, Is.True, "Model should still be in edit session after Apply so further edits are tracked");
        Assert.That(person.CalledEndEdit, Is.True, "EndEdit must have been called to commit the first Apply");
    }

    [TestCase]
    public async Task ModelsSavedTwiceBySaveViewModelAsync_SecondSaveSucceeds()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var person = new Person();
        person.FirstName = "first name";
        person.LastName = "last name";

        var viewModel = new TestFeaturedViewModel(person, serviceProvider);

        // First save (Apply)
        viewModel.FirstName = "apply first name";
        var firstSaveResult = await viewModel.SaveViewModelAsync();
        Assert.That(firstSaveResult, Is.True);
        Assert.That(person.FirstName, Is.EqualTo("apply first name"));

        // Second save (OK) - must also succeed
        viewModel.FirstName = "ok first name";
        var secondSaveResult = await viewModel.SaveViewModelAsync();
        Assert.That(secondSaveResult, Is.True, "Second save (OK after Apply) must succeed");
        Assert.That(person.FirstName, Is.EqualTo("ok first name"));

        await viewModel.CloseViewModelAsync(true);

        Assert.That(person.IsInEditSession, Is.False);
    }

    [TestCase]
    public async Task ModelsCanceledAfterSaveViewModelAsync_RevertsPostApplyChanges()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var person = new Person();
        person.FirstName = "first name";
        person.LastName = "last name";

        var viewModel = new TestFeaturedViewModel(person, serviceProvider);

        // Apply: commits "applied first name"
        viewModel.FirstName = "applied first name";
        var saveResult = await viewModel.SaveViewModelAsync();
        Assert.That(saveResult, Is.True);
        Assert.That(person.FirstName, Is.EqualTo("applied first name"));

        // Cancel (X / Cancel button) after Apply must cancel the post-Apply edits
        await viewModel.CancelAndCloseViewModelAsync();

        Assert.That(person.IsInEditSession, Is.False);
        Assert.That(person.CalledCancelEdit, Is.True, "CancelEdit must be called to roll back post-Apply changes");
    }

    [TestCase]
    public void IsModelRegistered_ExistingModel()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var person = new Person();
        person.FirstName = "first name";
        person.LastName = "last name";

        var viewModel = new TestFeaturedViewModel(person, serviceProvider);

        Assert.That(viewModel.IsModelRegisteredForTest("Person"), Is.True);
    }

    [TestCase]
    public void IsModelRegistered_NonExistingModel()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var person = new Person();
        person.FirstName = "first_name";
        person.LastName = "last_name";

        var viewModel = new TestFeaturedViewModel(person, serviceProvider);

        Assert.That(viewModel.IsModelRegisteredForTest("SecondPerson"), Is.False);
    }
}
